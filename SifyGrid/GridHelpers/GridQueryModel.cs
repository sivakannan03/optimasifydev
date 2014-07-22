namespace  SifyGrid.GridHelpers
{
    public class GridQueryModel
    {
        public bool _search { get; set; }
        public string searchString { get; set; }
        public string searchOper { get; set; }
        public string searchField { get; set; }
        public int rows { get; set; }
        public int page { get; set; }
        public string sord { get; set; }
        public string sidx { get; set; }
    }
}