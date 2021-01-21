namespace DefaultNamespace
{
    public interface IResettable
    {
        public void CreateSnapshot();

        public void ReloadSnapshot();
    }
}