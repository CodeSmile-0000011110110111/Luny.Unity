using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(nameof(Luny) + ".UnityEditor")]

// because of a Rider issue
[assembly: InternalsVisibleTo("Luny.UnityEditor")]
[assembly: InternalsVisibleTo("Luny-ContractTest")]
