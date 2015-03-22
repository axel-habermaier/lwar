#ifndef pgFileSystem_h__
#define pgFileSystem_h__

#include "pg.h"

#ifdef PG_COMPILER_VISUAL_STUDIO
	typedef wchar_t pgPathChar;
#else
	typedef pgChar pgPathChar;
#endif

PG_API_EXPORT pgBool pgReadAppFile(pgString path, pgByte* buffer, pgUInt32* sizeInBytes);
PG_API_EXPORT pgBool pgReadUserFile(pgString fileName, pgByte* buffer, pgUInt32* sizeInBytes);

PG_API_EXPORT pgBool pgWriteUserFile(pgString fileName, pgByte* content, pgUInt32 sizeInBytes);
PG_API_EXPORT pgBool pgAppendUserFile(pgString fileName, pgByte* content, pgUInt32 sizeInBytes);

PG_API_EXPORT pgBool pgDeleteUserFile(pgString fileName);
PG_API_EXPORT pgBool pgUserFileExists(pgString fileName);

PG_API_EXPORT const pgPathChar* pgGetUserDirectory();
PG_API_EXPORT pgString pgGetLastFileError();

#endif