namespace RStein.HDO.CEZ
{

  public class CezJsonRoot
  {
    public Datum[] data { get; set; }
    public int pageSize { get; set; }
    public int pageNumber { get; set; }
    public int pageOffset { get; set; }
    public int pageBarItems { get; set; }
    public int totalNumberOfRecords { get; set; }
    public Pagingmodel pagingModel { get; set; }
    public int firstRecordNumberForPage { get; set; }
    public bool onePage { get; set; }
    public int lastPageNumber { get; set; }
    public int lastRecordNumber { get; set; }
    public int[] pageNumbersNavigation { get; set; }
    public int[] defaultPageNumbersAround { get; set; }
    public bool fullyInitialized { get; set; }
    public bool last { get; set; }
    public bool first { get; set; }
  }

  public class Pagingmodel
  {
  }

  public class Datum
  {
    public int primaryKey { get; set; }
    public int ID { get; set; }
    public long VALID_FROM { get; set; }
    public long VALID_TO { get; set; }
    public int DUMP_ID { get; set; }
    public string POVEL { get; set; }
    public string KOD { get; set; }
    public string KOD_POVELU { get; set; }
    public string SAZBA { get; set; }
    public string INFO { get; set; }
    public string PLATNOST { get; set; }
    public string DOBA { get; set; }
    public string CAS_ZAP_1 { get; set; }
    public string CAS_VYP_1 { get; set; }
    public string CAS_ZAP_2 { get; set; }
    public string CAS_VYP_2 { get; set; }
    public string CAS_ZAP_3 { get; set; }
    public string CAS_VYP_3 { get; set; }
    public string CAS_ZAP_4 { get; set; }
    public string CAS_VYP_4 { get; set; }
    public string CAS_ZAP_5 { get; set; }
    public string CAS_VYP_5 { get; set; }
    public object CAS_ZAP_6 { get; set; }
    public object CAS_VYP_6 { get; set; }
    public object CAS_ZAP_7 { get; set; }
    public object CAS_VYP_7 { get; set; }
    public object CAS_ZAP_8 { get; set; }
    public object CAS_VYP_8 { get; set; }
    public object CAS_ZAP_9 { get; set; }
    public object CAS_VYP_9 { get; set; }
    public object CAS_ZAP_10 { get; set; }
    public object CAS_VYP_10 { get; set; }
    public long DATE_OF_ENTRY { get; set; }
    public string DESCRIPTION { get; set; }
  }

}