@echo off
REM Sample Blog Docker Management for Windows

if "%1"=="" goto help
if "%1"=="help" goto help
if "%1"=="up" goto up
if "%1"=="down" goto down
if "%1"=="build" goto build
if "%1"=="logs" goto logs
if "%1"=="clean" goto clean
if "%1"=="rebuild" goto rebuild
if "%1"=="shell" goto shell
if "%1"=="db-shell" goto dbshell
if "%1"=="backup" goto backup
if "%1"=="health" goto health

:help
echo Sample Blog Docker Commands:
echo   docker-run.bat up          - Start all services
echo   docker-run.bat down        - Stop all services
echo   docker-run.bat build       - Build all services
echo   docker-run.bat rebuild     - Rebuild and restart all services
echo   docker-run.bat logs        - Show logs
echo   docker-run.bat clean       - Remove containers and volumes
echo   docker-run.bat shell       - Access backend shell
echo   docker-run.bat db-shell    - Access database shell
echo   docker-run.bat backup      - Backup database
echo   docker-run.bat health      - Check service health
goto end

:up
echo Starting Sample Blog services...
docker-compose up -d
goto end

:down
echo Stopping Sample Blog services...
docker-compose down
goto end

:build
echo Building Sample Blog services...
docker-compose build
goto end

:rebuild
echo Rebuilding and restarting Sample Blog services...
docker-compose up -d --force-recreate --build
goto end

:logs
docker-compose logs -f
goto end

:clean
echo Cleaning up Sample Blog containers and volumes...
docker-compose down -v --remove-orphans
docker system prune -f
goto end

:shell
echo Accessing backend container shell...
docker-compose exec backend bash
goto end

:dbshell
echo Accessing PostgreSQL shell...
docker-compose exec postgres psql -U postgres -d SampleBlogDb
goto end

:backup
echo Creating database backup...
for /f "tokens=2 delims==" %%a in ('wmic OS Get localdatetime /value') do set "dt=%%a"
set "YY=%dt:~2,2%" & set "YYYY=%dt:~0,4%" & set "MM=%dt:~4,2%" & set "DD=%dt:~6,2%"
set "HH=%dt:~8,2%" & set "Min=%dt:~10,2%" & set "Sec=%dt:~12,2%"
set "timestamp=%YYYY%%MM%%DD%_%HH%%Min%%Sec%"
docker-compose exec postgres pg_dump -U postgres SampleBlogDb > backup_%timestamp%.sql
echo Backup completed: backup_%timestamp%.sql
goto end

:health
echo Checking service health...
docker-compose ps
echo.
echo Backend health:
curl -s http://localhost:8080/health
echo.
echo Database health:
docker-compose exec postgres pg_isready -U postgres
goto end

:end