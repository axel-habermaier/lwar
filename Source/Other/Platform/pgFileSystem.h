#ifndef pgFileSystem_h__
#define pgFileSystem_h__

#include "pg.h"

#ifdef _MSC_VER
	typedef wchar_t pgPathChar;
#else
	typedef pgChar pgPathChar;
#endif

PG_API_EXPORT pgBool pgReadAppFile(pgString path, pgByte* buffer, pgUint32* sizeInBytes);
PG_API_EXPORT pgBool pgReadUserFile(pgString fileName, pgByte* buffer, pgUint32* sizeInBytes);

PG_API_EXPORT pgBool pgWriteUserFile(pgString fileName, pgByte* content, pgUint32 sizeInBytes);
PG_API_EXPORT pgBool pgAppendUserFile(pgString fileName, pgByte* content, pgUint32 sizeInBytes);

PG_API_EXPORT pgBool pgDeleteUserFile(pgString fileName);

PG_API_EXPORT const pgPathChar* pgGetUserDirectory();
PG_API_EXPORT pgString pgGetLastFileError();

#endif