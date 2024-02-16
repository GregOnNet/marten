using Marten.Events.Projections;
using Marten.Internal.Storage;

namespace Marten.Events;

public interface IFetchPlanner
{
    bool TryMatch<TDoc, TId>(IDocumentStorage<TDoc, TId> storage, IEventIdentityStrategy<TId> identity,
        StoreOptions options, out IAggregateFetchPlan<TDoc, TId> plan) where TDoc : class;
}


internal class InlineFetchPlanner : IFetchPlanner
{
    public bool TryMatch<TDoc, TId>(IDocumentStorage<TDoc, TId> storage, IEventIdentityStrategy<TId> identity, StoreOptions options,
        out IAggregateFetchPlan<TDoc, TId> plan) where TDoc : class
    {
        if (options.Projections.TryFindAggregate(typeof(TDoc), out var projection))
        {
            if (projection.Lifecycle == ProjectionLifecycle.Inline)
            {
                plan = new FetchInlinedPlan<TDoc, TId>(options.EventGraph, identity, storage);
                return true;
            }
        }

        plan = default;
        return false;
    }
}

internal class LiveFetchPlanner: IFetchPlanner
{
    public bool TryMatch<TDoc, TId>(IDocumentStorage<TDoc, TId> storage, IEventIdentityStrategy<TId> identity,
        StoreOptions options, out IAggregateFetchPlan<TDoc, TId> plan) where TDoc : class
    {
        plan = new FetchLivePlan<TDoc, TId>(options.EventGraph, identity, storage);
        return true;
    }
}
