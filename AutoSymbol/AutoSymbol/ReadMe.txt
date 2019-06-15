Main Workflow
1. Start with any symbol chain (OpChain), using constructor can create different OpChain. Each one is a seed.
2. Apply EquivalentRelation to a seed, this process will develop many many equal OpChain, the key is to avoid explosion.
3. From seed, to the ideal result Chain, there will be Beauty criteria to limit exploring too many paths
4. Manual training will be required to build those Beauty criteria library
5. End goal is to let Beauty criteria to explore to find interesting equivalent relations

Optimization
1. To optimize beauty criteria, use manual approach so that I can discover factors visually