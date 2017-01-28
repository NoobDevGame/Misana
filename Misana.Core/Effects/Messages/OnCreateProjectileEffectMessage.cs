using Misana.Network;

namespace Misana.Core.Effects.Messages
{
    [MessageDefinition]
    public struct OnCreateProjectileEffectMessage
    {
        public float Speed;
        public float Radius;
        public float Expiration;
        public int area;

        public Vector2 move;
        public Vector2 position;

        public float Damage;

        public OnCreateProjectileEffectMessage(float speed, float radius, float expiration,int area, Vector2 move, Vector2 position,float damage)
        {
            Speed = speed;
            Radius = radius;
            Expiration = expiration;
            this.area = area;
            this.move = move;
            this.position = position;
            Damage = damage;
        }
    }
}