namespace Docomposer.Core.Domain.Settings
{
    public interface ISetting
    {
        public T GetSetting<T>();
        public void SetSetting<T>(T setting);
    }
}