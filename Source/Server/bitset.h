typedef unsigned long BitSet;

#define set_insert(s,i) \
    (s) |= (BitSet)(1 << (i))

#define set_remove(s,i) \
    (s) &= ~(BitSet)(1 << (i))

#define set_contains(s,i) \
    (((s) & (BitSet)(1 << (i))) != 0)
