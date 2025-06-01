#!/bin/bash

CONTAINER_NAME="mssql_server"
SA_PASSWORD="" # ТУТ НУЖНО УКАЗЫВАТЬ ПАРОЛЬ К MSSQL ПРИ ИСПОЛЬЗОВАНИЕ
DATABASES_TO_BACKUP=("LightPlay_EnigmaVault_Users"
                     "LightPlay_EnigmaVault_Vaults") 

BACKUP_DIR_HOST="/opt/mssql_backups"
BACKUP_DIR_CONTAINER="/var/opt/mssql/backups"

TIMESTAMP=$(date +"%Y-%m-%d_%H-%M-%S")
LOG_FILE="${BACKUP_DIR_HOST}/backup_log_$(date +%Y%m%d).log"
DAYS_TO_KEEP_BACKUPS=7

log_message()
{
    echo "$(date +"%Y-%m-%d %H:%M:%S") - $1" >> "$LOG_FILE"
}

log_message "--- Начало процесса бэкапирования ---"

if ! docker ps -q -f name="^/${CONTAINER_NAME}$";
 then
    log_message "ОШИБКА: Контейнер ${CONTAINER_NAME} не запущен."
    exit 1
fi

if [ ! -d "$BACKUP_DIR_HOST" ]; 
then
    log_message "ОШИБКА: Директория для бэкапов ${BACKUP_DIR_HOST} не найдена на хосте. Создайте её."
    exit 1
fi

for DB_NAME in "${DATABASES_TO_BACKUP[@]}"; 
do
    BACKUP_FILENAME="${DB_NAME}_${TIMESTAMP}.bak"
    BACKUP_FILE_PATH_CONTAINER="${BACKUP_DIR_CONTAINER}/${BACKUP_FILENAME}"

    log_message "Создание бэкапа для БД: ${DB_NAME} в файл ${BACKUP_FILE_PATH_CONTAINER}"

    docker exec -i "${CONTAINER_NAME}" /opt/mssql-tools18/bin/sqlcmd \
        -S localhost -U sa -P "${SA_PASSWORD}" -N -C \
        -Q "BACKUP DATABASE [${DB_NAME}] TO DISK = N'${BACKUP_FILE_PATH_CONTAINER}' WITH FORMAT, INIT, COMPRESSION, STATS = 10"

    if [ $? -eq 0 ]; 
    then
        log_message "Бэкап БД ${DB_NAME} успешно создан: ${BACKUP_DIR_HOST}/${BACKUP_FILENAME}"
    else
        log_message "ОШИБКА: Не удалось создать бэкап для БД ${DB_NAME}."
    fi
done

if [ "$DAYS_TO_KEEP_BACKUPS" -gt 0 ]; 
then
    log_message "Удаление бэкапов старше ${DAYS_TO_KEEP_BACKUPS} дней из ${BACKUP_DIR_HOST}..."
    find "${BACKUP_DIR_HOST}" -name "*.bak" -type f -mtime +"${DAYS_TO_KEEP_BACKUPS}" -print -delete >> "$LOG_FILE" 2>&1
    log_message "Очистка старых бэкапов завершена."
else
    log_message "Очистка старых бэкапов отключена (DAYS_TO_KEEP_BACKUPS=0)."
fi

log_message 
echo "Лог бэкапирования: ${LOG_FILE}"

exit 0


# Если будет ошибка - Operating system error 5(Access is denied.). То нужно изменить владельца директории на хост
#
# 1. Узнать UID пользователя mssql внутри контейнера
#       docker exec -it <<Имя или id контейнера>> id mssql.
#
# 2. Изменить владельца директории на хосте
#       sudo chown 10001:10001 /opt/mssql_backups - вводим UID тот же, что вывела предыдущая команда.
#
# Если исполняемый файл sqlcmd не найден. То можно найти его следующим способом
#
# 1. Зайти внутрь контейнера
#       docker exec -it mssql_server /bin/bash
# 
# 2. Внутри контейнера выполняем 
#       find / -name sqlcmd 2>/dev/null
# 
# 3. Меняем путь - 41 строка
#       docker exec -i "${CONTAINER_NAME}" /opt/mssql-tools18/bin/sqlcmd