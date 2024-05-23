using System.Linq;
using AutoFixture;

namespace MessageService.Web.FunctionalTests;

public static class FixtureGlobal
{
    public static Fixture Get()
    {
        var fixture = new Fixture();
        
        // Define Custom fixture behaviors here
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
            .ToList().ForEach(f => fixture.Behaviors.Remove(f));
        fixture.Behaviors.Add(new NullRecursionBehavior());
        
        // Define other customizations below

        return fixture;
    }
}