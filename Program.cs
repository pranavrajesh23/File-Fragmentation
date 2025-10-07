namespace FileFragmentationConsole
{
    class Program
    {
        static void Main()
        {
            var model = new FileFragmentationModel();
            var view = new FileFragmentationView();
            var controller = new FileFragmentationController(model, view);
            controller.StartApp();
        }
    }
}
