Operator lifecycle?
1. Junior Set fill different Slot for Senior Set
2. New operator will be defined on Senior Set
3. New operator will be OpTree to map slots from Junior set to Senior set
4. Each new operator will keep samples, by invoking each slot samples
5. Let new operator form many trees, check if exists ER

Signature of a formula
1. Each set member should have its signature, by concatenate base set members
2. Formula signature, includes input signature and output signature
3. ER needs to match both
4. To compare 2 formula, will need to lock input and its sequence

Operator construction complexity?
1. Total possible OpTree
2. Total possible EndNode to Slot mapping
3. Each node can be different operator
4. If quarternion or matrix input slot, possibility increases
5. Potential one slot can occupy multiple EndNode
6. Sheer size of all the possible operators
7. Sheer size of the formula can be combined from those operators
a. Cut down : each node will have default single operator, and limited variable operator
b. Cut down : always start from easy opTree
c. Cut down : Hand pick allowed everything to limit choices
d. cut down : every dimension has a manual priority list, combination rule expand based on that
e. cut down : rotate one dimension to give more opportunity
