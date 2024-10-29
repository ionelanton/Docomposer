namespace Docomposer.Data.Util.Localization
{
    public static partial class Localization
    {
        public static class Settings
        {
            public static string Language =>
                Lang switch
                {
                    Lng.Fr => "Langue",
                    _ => "Language"
                };

            public static string WordPath =>
                Lang switch
                {
                    Lng.Fr => "Chemin Microsoft Word",
                    _ => "Microsoft Word Path"
                };

            public static string ExcelPath =>
                Lang switch
                {
                    Lng.Fr => "Chemin Microsoft Word",
                    _ => "Microsoft Word Path"
                };
        }
    }
}