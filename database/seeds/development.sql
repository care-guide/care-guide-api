BEGIN;

DELETE FROM persons WHERE id IN (
    '10000000-0000-0000-0000-000000000001',
    '10000000-0000-0000-0000-000000000002'
);
DELETE FROM phones WHERE id IN (
    '20000000-0000-0000-0000-000000000001',
    '20000000-0000-0000-0000-000000000002',
    '20000000-0000-0000-0000-000000000003'
);
DELETE FROM addresses WHERE id IN (
    '30000000-0000-0000-0000-000000000001',
    '30000000-0000-0000-0000-000000000002'
);

INSERT INTO persons (id, name, gender, birthday, picture, created_at, updated_at) VALUES
('10000000-0000-0000-0000-000000000001', 'Maria Care Guide', 'F', '1988-05-14', NULL, NOW(), NOW()),
('10000000-0000-0000-0000-000000000002', 'Admin Care Guide', 'O', '1990-01-20', NULL, NOW(), NOW());

INSERT INTO users (id, person_id, email, password, role, created_at, updated_at) VALUES
('11000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'user@careguide.local', '{{USER_PASSWORD_HASH}}', 0, NOW(), NOW()),
('11000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'admin@careguide.local', '{{ADMIN_PASSWORD_HASH}}', 1, NOW(), NOW());

INSERT INTO refresh_tokens (id, user_id, token, expires_at, revoked, created_at, updated_at) VALUES
('12000000-0000-0000-0000-000000000001', '11000000-0000-0000-0000-000000000001', 'seed-user-revoked-refresh-token', NOW() - INTERVAL '1 day', TRUE, NOW(), NOW()),
('12000000-0000-0000-0000-000000000002', '11000000-0000-0000-0000-000000000002', 'seed-admin-revoked-refresh-token', NOW() - INTERVAL '1 day', TRUE, NOW(), NOW());

INSERT INTO person_healths (id, person_id, blood_type, height, weight, description, created_at, updated_at) VALUES
('13000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'O+', 1.65, 64.50, 'Paciente em acompanhamento clínico regular.', NOW(), NOW()),
('13000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'A+', 1.78, 78.00, 'Perfil administrativo com dados para teste de isolamento.', NOW(), NOW());

INSERT INTO phones (id, number, area_code, type, created_at, updated_at) VALUES
('20000000-0000-0000-0000-000000000001', '999998888', '11', 'CEL', NOW(), NOW()),
('20000000-0000-0000-0000-000000000002', '33334444', '11', 'COM', NOW(), NOW()),
('20000000-0000-0000-0000-000000000003', '988887777', '21', 'CEL', NOW(), NOW());

INSERT INTO person_phones (person_id, phone_id) VALUES
('10000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000001'),
('10000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000003');

INSERT INTO person_annotations (id, person_id, details, file_url, created_at, updated_at) VALUES
('14000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'Relatou boa adaptação ao tratamento.', NULL, NOW(), NOW()),
('14000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Anotação exclusiva do perfil administrativo.', NULL, NOW(), NOW());

INSERT INTO person_diseases (id, person_id, name, diagnosis_date, disease_type, created_at, updated_at) VALUES
('15000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'Hipertensão arterial', '2020-03-10', 'CAR', NOW(), NOW()),
('15000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Asma', '2018-08-05', 'RES', NOW(), NOW());

INSERT INTO person_family_histories (id, person_id, relationship, diagnosis, age_at_diagnosis, created_at, updated_at) VALUES
('16000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'Mãe', 'Diabetes tipo 2', 52, NOW(), NOW()),
('16000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Pai', 'Hipertensão arterial', 48, NOW(), NOW());

INSERT INTO doctors (id, person_id, name, details, created_at, updated_at) VALUES
('17000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'Dra. Ana Oncologista', 'CRM-SP 123456', NOW(), NOW()),
('17000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Dr. Bruno Clínico', 'CRM-RJ 654321', NOW(), NOW());

INSERT INTO doctor_specialties (id, doctor_id, name, created_at, updated_at) VALUES
('18000000-0000-0000-0000-000000000001', '17000000-0000-0000-0000-000000000001', 'Oncologia Clínica', NOW(), NOW()),
('18000000-0000-0000-0000-000000000002', '17000000-0000-0000-0000-000000000002', 'Clínica Médica', NOW(), NOW());

INSERT INTO addresses (id, postal_code, number, type, city, complement, country, district, reference_point, state, created_at, updated_at) VALUES
('30000000-0000-0000-0000-000000000001', '01310-100', 1000, 'CLI', 'São Paulo', '8º andar', 'Brasil', 'Bela Vista', 'Próximo ao metrô', 'SP', NOW(), NOW()),
('30000000-0000-0000-0000-000000000002', '20040-020', 50, 'URG', 'Rio de Janeiro', NULL, 'Brasil', 'Centro', NULL, 'RJ', NOW(), NOW());

INSERT INTO doctor_phones (doctor_id, phone_id) VALUES
('17000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000002');
INSERT INTO doctor_addresses (doctor_id, address_id) VALUES
('17000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000001'),
('17000000-0000-0000-0000-000000000002', '30000000-0000-0000-0000-000000000002');

INSERT INTO events (id, person_id, name, start_date, end_date, type, is_all_day, recurrence_type, recurrence_frequency, details, created_at, updated_at) VALUES
('40000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'Consulta de acompanhamento', NOW() + INTERVAL '7 days', NOW() + INTERVAL '7 days 1 hour', 'Appointment', FALSE, 'Appointment', 'Monthly', 'Consulta mensal.', NOW(), NOW()),
('40000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000001', 'Hemograma completo', NOW() + INTERVAL '10 days', NULL, 'Exam', TRUE, NULL, NULL, 'Exame laboratorial.', NOW(), NOW()),
('40000000-0000-0000-0000-000000000003', '10000000-0000-0000-0000-000000000001', 'Medicação diária', NOW() + INTERVAL '1 day', NULL, 'Medicine', FALSE, 'Medicine', 'Daily', 'Tomar após o café da manhã.', NOW(), NOW()),
('40000000-0000-0000-0000-000000000004', '10000000-0000-0000-0000-000000000001', 'Procedimento diagnóstico', NOW() + INTERVAL '30 days', NOW() + INTERVAL '30 days 2 hours', 'Surgery', FALSE, NULL, NULL, 'Procedimento agendado.', NOW(), NOW()),
('40000000-0000-0000-0000-000000000005', '10000000-0000-0000-0000-000000000002', 'Consulta administrativa de teste', NOW() + INTERVAL '14 days', NULL, 'Appointment', FALSE, NULL, NULL, 'Evento isolado do administrador.', NOW(), NOW());

INSERT INTO event_appointments (id, event_id, doctor, type, created_at, updated_at) VALUES
('41000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000001', 'Dra. Ana Oncologista', 'FOL', NOW(), NOW()),
('41000000-0000-0000-0000-000000000002', '40000000-0000-0000-0000-000000000005', 'Dr. Bruno Clínico', 'ROU', NOW(), NOW());
INSERT INTO event_exams (id, event_id, type, created_at, updated_at) VALUES
('42000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000002', 'BLO', NOW(), NOW());
INSERT INTO event_medicines (id, event_id, dosage, laboratory, created_at, updated_at) VALUES
('43000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000003', '1 comprimido de 8 em 8 horas', 'Care Pharma', NOW(), NOW());
INSERT INTO event_surgeries (id, event_id, type, created_at, updated_at) VALUES
('44000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000004', 'DIA', NOW(), NOW());

INSERT INTO event_addresses (event_id, address_id) VALUES
('40000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000001'),
('40000000-0000-0000-0000-000000000005', '30000000-0000-0000-0000-000000000002');

INSERT INTO files (id, person_id, name, size, hash, storage_key, content_type, storage_provider, upload_status, created_at, updated_at) VALUES
('50000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'hemograma.pdf', 245760, repeat('a', 64), 'files/50000000000000000000000000000001', 'application/pdf', 'CloudflareR2', 'Uploaded', NOW(), NOW()),
('50000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'admin-note.pdf', 10240, repeat('b', 64), 'files/50000000000000000000000000000002', 'application/pdf', 'CloudflareR2', 'Pending', NOW(), NOW());

INSERT INTO event_files (event_id, file_id) VALUES
('40000000-0000-0000-0000-000000000002', '50000000-0000-0000-0000-000000000001'),
('40000000-0000-0000-0000-000000000005', '50000000-0000-0000-0000-000000000002');

COMMIT;
