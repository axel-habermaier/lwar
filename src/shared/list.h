typedef struct List List;
typedef struct List
{
	List* next;
	List* prev;
} List;

#define LIST_INIT(l) { &(l), &(l) }

// Initializes the list head
void list_init_head(List* head);

// Adds node right after prevNode into the list
void list_add(List* node, List* prevNode);

// Removes the node from the list
void list_remove(List* node);

// Gets the list entry, where list is a pointer to a List, type is the type of the struct containing the list member
#define list_element(list, type, member) (type*)((uint8_t*)list - offsetof(type, member))

// Iterates through the list starting at head, where node is set to the current node during each iteration
// During iteration, it is NOT safe to add and remove elements from the list
#define list_foreach(node, head) for (node = (head)->next; node != (head); node = node->next)