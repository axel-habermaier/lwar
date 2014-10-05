#include "message.h"

#include "uint.h"
#include "debug.h"
#include "log.h"

#include <stdlib.h>

bool has_seqno(Message *m) {
	return m->type != MESSAGE_DISCOVERY;
}

bool is_reliable(Message *m) {
    return m->type < 100;
}

