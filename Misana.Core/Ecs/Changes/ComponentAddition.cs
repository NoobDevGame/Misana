namespace Misana.Core.Ecs.Changes
{
    public abstract class ComponentAddition<T> : EntityChange where T : Component, new()
    {
        protected ComponentAddition(int entityId) : base(entityId, ComponentRegistry<T>.Index) { }
        protected ComponentAddition() : base  (ComponentRegistry<T>.Index) { }
    }
}