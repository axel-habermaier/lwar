#include "prelude.h"

int main()
{
	log_init();
	sys_init();

	LWAR_ERROR("THIS IS AN ERROR! %s", "blub");
	LWAR_WARN("WARNIIING! %s", "blub");
	LWAR_INFO("INF! %s", "blub");
	LWAR_DEBUG("DEBUUUG! %s", "blub");
	LWAR_DIE("DDIIIIEEEE! %s", "blub");
}
