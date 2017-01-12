using Misana.Core.Components;
using Misana.Core.Components.StatusComponent;
using Misana.Core.Ecs;

namespace Misana.Core.Systems.StatusSystem
{
    public class TimeDamageSystem : BaseSystemR2<TimeDamageComponent,HealthComponent>
    {
        public override void Tick()
        {
            for (int i = 0; i < Count; i++)
            {
                var r1 = R1S[i];
                var r2 = R2S[i];


                r2.Current -= r1.DamagePerSeconds * (float)GameTime.ElapsedTime.TotalSeconds;
                if ((r1.CurrentTime += GameTime.ElapsedTime) > r1.EffectTime)
                {
                    Entities[i--].Remove<TimeDamageComponent>();
                }
            }
        }
    }
}