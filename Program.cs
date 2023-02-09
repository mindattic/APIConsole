using APIConsole.Extensions;
using APIConsole.Utilities;
using Request;
using Response;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace APIConsole
{
    static class Program
    {
        //Configuration
        const int THREAD_COUNT = 100;
        const bool UPDATE_UI = true;
        const int UPDATE_RATE = 1000;
        const int TASK_DELAY = 1;
        const string BASE_URL = "https://secure.shippingapis.com/ShippingAPI.dll?API=Verify&XML=";
        const string USER_ID = "213CVSHE8041";
        const string INPUT_FILE = "test1.csv";
        const string OUTPUT_FILE = "results.csv";

        //Startup
        static async Task Main(string[] args)
        {
            Load();
            Build();
            await Run();
            Complete();
        }

        #region Variables

        //Constants
        const string TAB = "     ";
        static readonly Encoding ISO88591 = Encoding.GetEncoding("ISO-8859-1");

        //Utilities
        static readonly PrintUtility print = new PrintUtility();
        static readonly FileUtility file = new FileUtility(OUTPUT_FILE);

        //API 
        static HttpClient httpClient = new HttpClient();
        static Stopwatch watch = new Stopwatch();
        static List<Task> tasks = new List<Task>();
        static SemaphoreSlim throttler = new SemaphoreSlim(THREAD_COUNT);

        //XML
        static XmlSerializer xmlConvert = new XmlSerializer(typeof(AddressValidateResponse));

        //Properties
        static TimeSpan TimeElasped => TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds);
        static string TimeStamp => $"{TimeElasped:hh\\:mm\\:ss}";

        //Variables
        static DataTable table;
        static List<AddressValidateRequest> requestList = new List<AddressValidateRequest>();
        static ConcurrentBag<HttpResponseMessage> responses = new ConcurrentBag<HttpResponseMessage>();
        static List<string> results = new List<string>();
        static int count = 0;

        #endregion

        private static void Load()
        {
            table = new CSVUtility(INPUT_FILE, false).Table;
        }

        private static void Build()
        {
            const int ADDRESS1 = 0;
            const int ADDRESS2 = 1;
            const int CITY = 2;
            const int STATE = 3;
            const int ZIP5 = 4;
            const int ZIP4 = 5;

            foreach (DataRow row in table.Rows)
            {
                var rq = new AddressValidateRequest();
                rq.Address.Address1 = (string)row.ItemArray[ADDRESS1] ?? "";
                rq.Address.Address2 = (string)row.ItemArray[ADDRESS2] ?? "";
                rq.Address.City = (string)row.ItemArray[CITY] ?? "";
                rq.Address.State = (string)row.ItemArray[STATE] ?? "";
                rq.Address.Zip5 = (string)row.ItemArray[ZIP5] ?? "";
                rq.Address.Zip4 = (string)row.ItemArray[ZIP4] ?? "";
                requestList.Add(rq);
            }
        }

        private static async Task Run()
        {
            watch.Reset();
            watch.Start();

            print.Line();
            print.Line($"{TimeStamp}{TAB}Starting...");
            print.Line();

            foreach (var rq in requestList)
            {
                await throttler.WaitAsync();

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var encodedXML = ComposeXML(rq);
                        var response = await GET(encodedXML);
                        if (!IsValidResponse(response))
                            return;

                        responses.Add(response);
                        Interlocked.Increment(ref count);
                        RefreshUI();

                        await Task.Delay(TASK_DELAY);
                    }
                    finally
                    {
                        throttler.Release();
                    }
                }));
            }

            //Wait for all the tasks to complete
            await Task.WhenAll(tasks.ToArray());

            watch.Stop();
        }

        private static async Task<HttpResponseMessage> GET(string encodedXML)
        {
            return await httpClient.GetAsync($"{BASE_URL}{encodedXML}");
        }

        private static string ComposeXML(AddressValidateRequest rq)
        {
            string xml
                = $@"<AddressValidateRequest USERID=""{USER_ID}"">"
                + $@"<Revision>1</Revision>"
                + $@"<Address ID=""0"">"
                + $@"<Address1>{rq.Address.Address1}</Address1>"
                + $@"<Address2>{rq.Address.Address2}</Address2>"
                + $@"<City>{rq.Address.City}</City>"
                + $@"<State>{rq.Address.State}</State>"
                + $@"<Zip5>{rq.Address.Zip5}</Zip5>"
                + $@"<Zip4>{rq.Address.Zip4}</Zip4>"
                + $@"</Address>"
                + $@"</AddressValidateRequest>";

            return HttpUtility.UrlEncode(xml, ISO88591);
        }

        private static bool IsValidResponse(HttpResponseMessage response)
        {
            return response != null && response.IsSuccessStatusCode && response.Content != null;
        }

        private static AddressValidateResponse ParseResponse(HttpResponseMessage response)
        {
            var content = response.Content.ReadAsStringAsync().Result;
            if (string.IsNullOrWhiteSpace(content) || content.Contains("<Error>")) return null;

            AddressValidateResponse rs;
            using (StringReader reader = new StringReader(content))
                rs = (AddressValidateResponse)xmlConvert.Deserialize(reader);

            return rs;
        }

        private static void RefreshUI()
        {
            if (UPDATE_UI && count % UPDATE_RATE == 0)
                print.Line($"{TimeStamp}{TAB}{count:N0} requests completed.");
        }

        private static void Complete()
        {
            //Iterate across all responses and generate results
            foreach (var response in responses)
            {
                if (!IsValidResponse(response))
                    return;

                //Convert response into <AddressValidateResponse> object
                AddressValidateResponse rs = ParseResponse(response);

                //Add result to collection
                WriteResult(rs);
            }

            print.Line();
            print.Line($"{TimeStamp}{TAB}Done.");
            print.Line();

            SaveResults();
            PrintResults();
        }

        private static void WriteResult(AddressValidateResponse rs)
        {
            var result = $"{rs.Address.Address1},{rs.Address.Address2},{rs.Address.City},{rs.Address.State},{rs.Address.Zip5},{rs.Address.Zip4}";
            results.Add(result);
        }

        private static void SaveResults()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var r in results)
            {
                sb.AppendLine(r);
            }

            file.Save(sb.ToString());
            file.Open();
        }

        private static void PrintResults()
        {
            print.Line($"{TimeStamp}{TAB}{results.Count:N0} requests completed in {TimeElasped.ToFriendlyDisplay(3)}.");
        }

    }
}
