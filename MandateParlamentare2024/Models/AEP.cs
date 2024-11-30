namespace MandateParlamentare2024.Models
{
    public class Field
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Candidate
    {
        public string Id { get; set; }
        public string CandidateName { get; set; }
        public string Party { get; set; }
        public string Votes { get; set; }
    }

    public class Table
    {
        public List<Field> Fields { get; set; }
        public string ReportId { get; set; }
        public string ReportCreatedAt { get; set; }
        public int ReportVersion { get; set; }
        public string PrecinctId { get; set; }
        public string PrecinctNr { get; set; }
        public string PrecinctName { get; set; }
        public string UatId { get; set; }
        public string UatName { get; set; }
        public string UatSiruta { get; set; }
        public string CountyId { get; set; }
        public string CountyCode { get; set; }
        public string CountyNce { get; set; }
        public string CountyName { get; set; }
        public string PrecinctCountyId { get; set; }
        public string PrecinctCountyCode { get; set; }
        public string PrecinctCountyName { get; set; }
        public string PrecinctCountyNce { get; set; }
        public string ReportTypeScopeCode { get; set; }
        public string ReportTypeCode { get; set; }
        public string ReportTypeCategoryCode { get; set; }
        public string ReportStageCode { get; set; }
        public List<Candidate> Candidates { get; set; }
    }

    public class Category
    {
        public Dictionary<string, Table> Table { get; set; }
    }

    public class Scope
    {
        public Dictionary<string, Category> Categories { get; set; }
    }

    public class Root
    {
        public Dictionary<string, Scope> Scopes { get; set; }
    }
}
