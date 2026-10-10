using System.Collections.Generic;

public static class ParticleKeySearcher
{
    public static IEnumerable<string> GetAllKeys()
    {
        return ParticlesLibrary.GetAllKeys();
    }
}