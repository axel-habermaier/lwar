#include "prelude.h"

void list_init_head(ListNode* head)
{
	LWAR_ASSERT_NOT_NULL(head);
	LWAR_ASSERT_NULL(head->next);
	LWAR_ASSERT_NULL(head->prev);

	head->next = head;
	head->prev = head;
}

void list_add(ListNode* node, ListNode* prevNode)
{
	LWAR_ASSERT_NOT_NULL(node);
	LWAR_ASSERT_NOT_NULL(prevNode);
	LWAR_ASSERT_NULL(node->next);
	LWAR_ASSERT_NULL(node->prev);
	LWAR_ASSERT_NOT_NULL(prevNode->next);
	LWAR_ASSERT_NOT_NULL(prevNode->prev);

	node->next = prevNode->next;
	node->prev = prevNode;
	prevNode->next = node;
}

void list_remove(ListNode* node)
{
	LWAR_ASSERT_NOT_NULL(node);
	LWAR_ASSERT_NOT_NULL(node->next);
	LWAR_ASSERT_NOT_NULL(node->prev);

	node->next->prev = node->prev;
	node->prev->next = node->next;
	node->next = NULL;
	node->prev = NULL;
}