#include "prelude.h"
#include <stdio.h>

#ifdef PG_COMPILER_VISUAL_STUDIO
	PG_DISABLE_WARNING(4917)
		#undef INITGUID
		#include <Shlobj.h>
	PG_ENABLE_WARNING(4917)
#endif

//====================================================================================================================
// Helper functions and error state
//====================================================================================================================

#ifdef PG_COMPILER_VISUAL_STUDIO
	#define fopen _wfopen
	#define remove _wremove
	#define MODE(x) L#x
#else
	#define MODE(x) #x
#endif

typedef enum pgFileMode
{
	PG_FILE_READ,
	PG_FILE_WRITE,
	PG_FILE_APPEND
} pgFileMode;

static pgBool hasError = PG_FALSE;
static pgChar lastError[2048];
static pgPathChar pathBuffer[FILENAME_MAX];

static pgVoid pgFileSystemError(pgString message);
static PG_NORETURN pgVoid pgFileSystemDie(pgString message);

static pgVoid pgSetPath(pgString path);
static pgVoid pgSetUserFilePath(pgString fileName);
static pgVoid pgAddToPath(pgString path);

static pgBool pgIsPathSeparator(pgPathChar character);
static pgVoid pgNormalize();

static FILE* pgOpenFile(pgFileMode fileMode);
static pgBool pgCloseFile(FILE* file);

static pgBool pgReadFile(pgByte* buffer, pgUInt32* sizeInBytes);
static pgBool pgWriteFile(pgFileMode fileMode, pgByte* content, pgUInt32 sizeInBytes);
static pgBool pgTryGetFileSize(FILE* file, pgUInt32* size);

//====================================================================================================================
// File system functions
//====================================================================================================================

PG_API_EXPORT pgBool pgReadAppFile(pgString path, pgByte* buffer, pgUInt32* sizeInBytes)
{
	PG_ASSERT_NOT_NULL(path);

	pgSetPath(path);
	return pgReadFile(buffer, sizeInBytes);
}

PG_API_EXPORT pgBool pgReadUserFile(pgString fileName, pgByte* buffer, pgUInt32* sizeInBytes)
{
	PG_ASSERT_NOT_NULL(fileName);

	pgSetUserFilePath(fileName);
	return pgReadFile(buffer, sizeInBytes);
}

PG_API_EXPORT pgBool pgWriteUserFile(pgString fileName, pgByte* content, pgUInt32 sizeInBytes)
{
	PG_ASSERT_NOT_NULL(fileName);

	pgSetUserFilePath(fileName);
	return pgWriteFile(PG_FILE_WRITE, content, sizeInBytes);
}

PG_API_EXPORT pgBool pgAppendUserFile(pgString fileName, pgByte* content, pgUInt32 sizeInBytes)
{
	PG_ASSERT_NOT_NULL(fileName);

	pgSetUserFilePath(fileName);
	return pgWriteFile(PG_FILE_APPEND, content, sizeInBytes);
}

PG_API_EXPORT pgBool pgUserFileExists(pgString fileName)
{
	FILE* file;

	PG_ASSERT_NOT_NULL(fileName);

	pgSetUserFilePath(fileName);
	file = fopen(pathBuffer, MODE(r));
	if (file == NULL)
		return PG_FALSE;

	fclose(file);
	return PG_TRUE;
}

PG_API_EXPORT pgBool pgDeleteUserFile(pgString fileName)
{
	PG_ASSERT_NOT_NULL(fileName);

	if (pgUserFileExists(fileName) && remove(pathBuffer) != 0)
	{
		pgFileSystemError("Failed to remove file.");
		return PG_FALSE;
	}

	return PG_TRUE;
}

PG_API_EXPORT const pgPathChar* pgGetUserDirectory()
{
	// On Windows, when compiling with Visual Studio, use the local app data directory to store app files; on all
	// other platforms and compilers, use the current working directory instead.

#ifdef PG_COMPILER_VISUAL_STUDIO
	PWSTR userDir;
	size_t length;

	if (SHGetKnownFolderPath(&FOLDERID_LocalAppData, 0, NULL, &userDir) != S_OK)
		pgFileSystemDie("Unable to get local app data folder.");

	length = wcslen(userDir);
	memcpy(pathBuffer, userDir, (length + 1) * sizeof(pgPathChar));
	pgAddToPath(pgState.appName);

	CoTaskMemFree(userDir);

	// Check if the directory exists, and if not, create it
	DWORD attributes = GetFileAttributesW(pathBuffer);
	pgBool exists = attributes != INVALID_FILE_ATTRIBUTES && attributes & FILE_ATTRIBUTE_DIRECTORY;

	if (!exists && !CreateDirectoryW(pathBuffer, NULL))
		pgFileSystemDie("Failed to create app directory.");
#else
	pathBuffer[0] = '\0';
#endif

	pgNormalize();
	return pathBuffer;
}

PG_API_EXPORT pgString pgGetLastFileError()
{
	if (!hasError)
		return NULL;

	hasError = PG_FALSE;
	return lastError;
}

//====================================================================================================================
// Helper functions
//====================================================================================================================

static pgVoid pgFileSystemError(pgString message)
{
	pgString osMessage = pgGetOsErrorMessage();
	sprintf(lastError, "%s %s", message, osMessage);
	hasError = PG_TRUE;
}

static PG_NORETURN pgVoid pgFileSystemDie(pgString message)
{
	pgFileSystemError(message);
	PG_DIE("%s", lastError);
}

static pgVoid pgSetPath(pgString path)
{
	pathBuffer[0] = '\0';
	pgAddToPath(path);
}

static pgVoid pgSetUserFilePath(pgString fileName)
{
	pgGetUserDirectory();
	pgAddToPath(fileName);
}

static pgVoid pgAddToPath(pgString path)
{
	pgPathChar* path1 = pathBuffer;
	const pgPathChar* path2;
	size_t length1;
	size_t length2;
	size_t length;

#ifdef PG_COMPILER_VISUAL_STUDIO
	pgPathChar conversionBuffer[FILENAME_MAX];
	size_t converted = mbstowcs(conversionBuffer, path, sizeof(conversionBuffer) / sizeof(pgPathChar));
	if (converted == (size_t)-1)
		PG_DIE("Failed to convert path into multi-byte format.");

	conversionBuffer[converted] = '\0';
	path2 = conversionBuffer;
	length1 = wcslen(path1);
	length2 = wcslen(path2);
#else
	path2 = path;
	length1 = strlen(path1);
	length2 = strlen(path2);
#endif

	if (length1 + length2 + 2 > FILENAME_MAX)
		PG_DIE("Paths too long.");

	if (length1 == 0 && length2 == 0)
	{
		path1[0] = '\0';
		return;
	}
	else if (length1 == 0)
	{
		memcpy(path1, path2, (length2 + 1) * sizeof(pgPathChar));
		return;
	}
	else if (length2 == 0)
		return;

	// Determine the length of the first path, not including the trailing path separator, if any
	length = pgIsPathSeparator(path1[length1 - 1]) ? length1 - 1 : length1;

	// Add the path separator
	path1[length++] = '/';

	// Copy the second path, not including the starting path separator, if any
	if (pgIsPathSeparator(path2[0]))
	{
		memcpy(&path1[length], path2 + 1, (length2 - 1) * sizeof(pgPathChar));
		length += length2 - 1;
	}
	else
	{
		memcpy(&path1[length], path2, length2 * sizeof(pgPathChar));
		length += length2;
	}

	path1[length] = '\0';
}

static pgBool pgIsPathSeparator(pgPathChar character)
{
	return character == '/' || character == '\\';
}

static pgVoid pgNormalize()
{
	int i;
	for (i = 0; pathBuffer[i] != '\0'; ++i)
	{
		if (pathBuffer[i] == '\\')
			pathBuffer[i] = '/';
	}
}

static FILE* pgOpenFile(pgFileMode fileMode)
{
	const pgPathChar* mode;
	FILE* file;

	switch (fileMode)
	{
	case PG_FILE_APPEND:
		mode = MODE(a);
		break;
	case PG_FILE_READ:
#ifdef PG_COMPILER_VISUAL_STUDIO
		mode = MODE(rb);
#else
		mode = MODE(r);
#endif
		break;
	case PG_FILE_WRITE:
		mode = MODE(w);
		break;
	default:
		PG_NO_SWITCH_DEFAULT;
	}

	file = fopen(pathBuffer, mode);
	if (file == NULL)
		pgFileSystemError("Failed to open file.");

	return file;
}

static pgBool pgCloseFile(FILE* file)
{
	if (file != NULL && fclose(file) != 0)
	{
		pgFileSystemError("Failed to close file.");
		return PG_FALSE;
	}

	return PG_TRUE;
}

static pgBool pgReadFile(pgByte* buffer, pgUInt32* sizeInBytes)
{
	FILE* file;

	PG_ASSERT_NOT_NULL(buffer);
	PG_ASSERT_NOT_NULL(sizeInBytes);

	file = pgOpenFile(PG_FILE_READ);
	if (file == NULL)
		return PG_FALSE;

	if (!pgTryGetFileSize(file, sizeInBytes))
	{
		pgCloseFile(file);
		return PG_FALSE;
	}

	if (fread(buffer, 1, *sizeInBytes, file) != *sizeInBytes)
	{
		pgCloseFile(file);
		pgFileSystemError("Failed to read the contents of the file.");
		return PG_FALSE;
	}

	pgCloseFile(file);
	return PG_TRUE;
}

static pgBool pgWriteFile(pgFileMode fileMode, pgByte* content, pgUInt32 sizeInBytes)
{
	FILE* file;

	PG_ASSERT(content != NULL || sizeInBytes == 0, "No file content has been specified.");

	file = pgOpenFile(fileMode);
	if (file == NULL)
		return PG_FALSE;

	if (sizeInBytes == 0)
	{
		pgCloseFile(file);
		return PG_TRUE;
	}

	if (fwrite(content, 1, sizeInBytes, file) != sizeInBytes)
	{
		pgCloseFile(file);
		pgFileSystemError("Failed to write to the file.");
		return PG_FALSE;
	}

	pgCloseFile(file);
	return PG_TRUE;
}

static pgBool pgTryGetFileSize(FILE* file, pgUInt32* size)
{
	long fileSize;

	if (fseek(file, 0L, SEEK_END) != 0)
	{
		pgFileSystemError("Failed to seek the end of the file.");
		return PG_FALSE;
	}

	fileSize = ftell(file);
	if (fileSize == -1L)
	{
		pgFileSystemError("Unable to determine the size of the file.");
		return PG_FALSE;
	}

	if ((pgInt64)fileSize > *size)
	{
		hasError = PG_TRUE;
		strcpy(lastError, "File is too large and cannot be opened.");
		return PG_FALSE;
	}

	if (fseek(file, 0L, SEEK_SET) != 0)
	{
		pgFileSystemError("Failed to seek the beginning of the file.");
		return PG_FALSE;
	}

	*size = (pgUInt32)fileSize;
	return PG_TRUE;
}