namespace BatchCutting_DG.A_Domain.Interfaces.Services.App
{
    public interface IS_Window
    {
        int GetMainWindowWidth();
        void SetMainWindowWidth(int value);

        int GetMainWindowHeight();
        void SetMainWindowHeight(int value);

        int GetMainWindowMinWidth();

        int GetMainWindowMinHeight();

        int GetMainWindowMarginAjusted();
        void SetMainWindowMarginAjusted(int value);

        double GetMainWindowTop();
        void SetMainWindowTop(double value);

        double GetMainWindowLeft();
        void SetMainWindowLeft(double value);

        void UpdateWindowDimensions(MainWindow mainWindow);

    }
}
