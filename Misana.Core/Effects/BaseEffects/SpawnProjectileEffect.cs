using System;
using System.IO;
using Misana.Core.Components;
using Misana.Core.Ecs;

namespace Misana.Core.Effects.BaseEffects
{
    public class SpawnProjectileEffect : EffectDefinition
    {
        public float Speed;
        public float Radius;
        public float Expiration;

        public SpawnProjectileEffect()
        {
            Radius = 0.3f;
            Expiration = 1500;
            Speed = 0.25f;
        }

        public override void Apply(Entity entity, ISimulation simulation)
        {
            Vector2 direction;

            var facingComponent = entity.Get<FacingComponent>();
            var transform = entity.Get<TransformComponent>();

            if (facingComponent == null)
            {
                var motion = entity.Get<MotionComponent>();
            
                if(motion == null)
                    throw new InvalidOperationException();

                direction = motion.Move;
            }
            else
            {
                direction = facingComponent.Facing;
            }

            if(transform == null)
                throw new InvalidOperationException();

            direction.Normalize();
            var move = direction * Speed;

            var offset = (direction * Radius) + move;

            if (transform.ParentEntityId != 0)
            {
                var w = entity.Get<WieldedComponent>();
                if (w != null)
                    offset += w.ParentPosition;
            }

            if (simulation.Mode == SimulationMode.SinglePlayer)
            {
                EntityBuilder builder = new EntityBuilder();
                builder.Add<MotionComponent>(x => x.Move = move * simulation.Entities.GameTime.ElapsedTime.TotalSeconds)
                    .Add<ProjectileComponent>(x => x.Move = move)
                    .Add<SpriteInfoComponent>()
                    .Add<TransformComponent>(t => {
                        t.CurrentArea = transform.CurrentArea;
                        t.Radius = Radius;
                        t.Position = transform.Position +  offset;
                    })
                    ;

                if (Expiration > 0)
                    builder.Add<ExpiringComponent>(x => x.TimeLeft = TimeSpan.FromMilliseconds(Expiration));

                builder.Commit(entity.Manager);

                return;
            }

            /*
            var builder = Builder.Copy()
                .Add<MotionComponent>(x => x.Move = move * simulation.Entities.GameTime.ElapsedTime.TotalSeconds)
                .Add<ProjectileComponent>(x => x.Move = move)
                .Add<TransformComponent>(t => {
                    t.CurrentArea = transform.CurrentArea;
                    t.Radius = Radius;
                    t.Position = transform.Position +  offset;
                })
                ;

            if (Expiration > 0)
                builder.Add<ExpiringComponent>(x => x.TimeLeft = TimeSpan.FromMilliseconds(Expiration));

            builder.Commit(entity.Manager);
            */
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {

        }

        public override void Deserialize(Version version, BinaryReader br)
        {

        }
    }
}