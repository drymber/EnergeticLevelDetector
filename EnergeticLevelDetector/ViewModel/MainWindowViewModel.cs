using EnergeticLevelDetector.Service;

namespace EnergeticLevelDetector.ViewModel
{
    public class MainWindowViewModel 
    {
        private GraphicRepository _graphicRepository = new GraphicRepository();
        private void GetGraphics()
        {
            _graphicRepository.GetAll();
            _graphicRepository.Serialize("Graphics.dat");
        }
    }
}
