namespace AppBinForm.Servies
{
    public interface IParamNavigationService<TParameter>
    {
        void Navigate(TParameter parameter);
    }
}