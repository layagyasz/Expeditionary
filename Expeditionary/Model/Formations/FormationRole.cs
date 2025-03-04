using System.Text.Json.Serialization;

namespace Expeditionary.Model.Formations
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FormationRole
    {
        None,

        Infantry,
        Engineer,

        Tank,
        TankDestroyer,
        AssaultGun,
        Scout,

        Artillery,
        
        Transport,
        Tractor,

        Support1,
        Support2,
        Support3,

        Special1,
        Special2,
        Special3
    }
}
