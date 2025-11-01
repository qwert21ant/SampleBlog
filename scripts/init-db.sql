-- Initialize SampleBlog Database
-- This script will be executed when the PostgreSQL container starts

-- Ensure the database exists (PostgreSQL will create it via POSTGRES_DB env var)
-- This file is mainly for any additional setup if needed

-- Enable extensions if needed
-- CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- The application will handle table creation via Entity Framework
SELECT 'blogdb database initialization complete' as message;