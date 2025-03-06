namespace STARTtoIFC
{
    public static class LanguageConverter
    {
        public static int ConvertLanguage(int languageId)
        {
            return languageId switch
            {
                1033 => 0x0409,
                1049 => 0x0419,
                _ => 0x0409
            };
        }
    }
}