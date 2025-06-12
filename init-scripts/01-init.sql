-- Create database if it doesn't exist
-- (This is handled by POSTGRES_DB environment variable)

-- Create exam questions table
CREATE TABLE IF NOT EXISTS exam_questions (
    id SERIAL PRIMARY KEY,
    question_number INTEGER NOT NULL,
    question_text TEXT NOT NULL,
    option1 VARCHAR(500) NOT NULL,
    option2 VARCHAR(500) NOT NULL,
    option3 VARCHAR(500) NOT NULL,
    option4 VARCHAR(500) NOT NULL,
    correct_answer INTEGER NOT NULL CHECK (correct_answer BETWEEN 1 AND 4),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Insert sample exam questions
INSERT INTO exam_questions (question_number, question_text, option1, option2, option3, option4, correct_answer) VALUES
    (1, 'ข้อใดต่อไปนี้คือคำตอบที่ถูกต้อง', '3', '5', '9', '11', 1),
    (2, 'x + 2 = 4 จงหาค่า x', '1', '2', '3', '4', 2)
ON CONFLICT DO NOTHING;

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_exam_questions_number ON exam_questions(question_number);

-- Create a sample table for demonstration
CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Create a sample table for products
CREATE TABLE IF NOT EXISTS products (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    price DECIMAL(10, 2) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Insert sample data
INSERT INTO users (username, email) VALUES
    ('admin', 'admin@example.com'),
    ('user1', 'user1@example.com'),
    ('user2', 'user2@example.com')
ON CONFLICT (username) DO NOTHING;

INSERT INTO products (name, description, price) VALUES
    ('Sample Product 1', 'This is a sample product description', 29.99),
    ('Sample Product 2', 'Another sample product', 49.99),
    ('Sample Product 3', 'Yet another sample product', 19.99)
ON CONFLICT DO NOTHING;

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_users_email ON users(email);
CREATE INDEX IF NOT EXISTS idx_products_name ON products(name);