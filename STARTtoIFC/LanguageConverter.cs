namespace STARTtoIFC
{
    public static class LanguageConverter
    {
        public static CLIDLanguage ConvertLanguage(StartLanguage startLanguage)
        {
            return startLanguage switch
            {
                StartLanguage.RU => CLIDLanguage.RU,
                StartLanguage.EN => CLIDLanguage.EN,
                _ => CLIDLanguage.EN
            };
        }

        public static StartLanguage ConvertLanguage(CLIDLanguage clidLanguage)
        {
            return clidLanguage switch
            {
                CLIDLanguage.RU => StartLanguage.RU,
                CLIDLanguage.EN => StartLanguage.EN,
                _ => StartLanguage.EN
            };
        }
    }
}