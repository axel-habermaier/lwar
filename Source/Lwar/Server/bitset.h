#ifndef BITSET_H
#define BITSET_H

typedef unsigned long BitSet;

#define set_empty 0

#define set_insert(s,i) \
    (s) |= (BitSet)(1 << (i))

#define set_remove(s,i) \
    (s) &= ~(BitSet)(1 << (i))

#define set_contains(s,i) \
    (((s) & (BitSet)(1 << (i))) != 0)

#define set_intersection(s1,s2) \
    ((s1) & (s2))

#define set_union(s1,s2) \
    ((s1) | (s2))

#define set_disjoint(s1,s2) \
    (set_intersection(s1,s2) == set_empty)

#endif
