-- Remove legacy ClinicaSim MVP seed cases and all dependent data.
-- Run on PostgreSQL.

BEGIN;

WITH seed_cases AS (
    SELECT id
    FROM clinical_cases
    WHERE full_name IN (
        'Paciente Ficticio 001',
        'Paciente Ficticio 002',
        'Paciente Ficticio 003'
    )
),
seed_sessions AS (
    SELECT id
    FROM consultation_sessions
    WHERE case_id IN (SELECT id FROM seed_cases)
)
DELETE FROM interaction_events
WHERE session_id IN (SELECT id FROM seed_sessions);

WITH seed_cases AS (
    SELECT id
    FROM clinical_cases
    WHERE full_name IN (
        'Paciente Ficticio 001',
        'Paciente Ficticio 002',
        'Paciente Ficticio 003'
    )
),
seed_sessions AS (
    SELECT id
    FROM consultation_sessions
    WHERE case_id IN (SELECT id FROM seed_cases)
)
DELETE FROM differential_diagnoses
WHERE session_id IN (SELECT id FROM seed_sessions);

WITH seed_cases AS (
    SELECT id
    FROM clinical_cases
    WHERE full_name IN (
        'Paciente Ficticio 001',
        'Paciente Ficticio 002',
        'Paciente Ficticio 003'
    )
),
seed_sessions AS (
    SELECT id
    FROM consultation_sessions
    WHERE case_id IN (SELECT id FROM seed_cases)
)
DELETE FROM clinical_notes
WHERE session_id IN (SELECT id FROM seed_sessions);

WITH seed_cases AS (
    SELECT id
    FROM clinical_cases
    WHERE full_name IN (
        'Paciente Ficticio 001',
        'Paciente Ficticio 002',
        'Paciente Ficticio 003'
    )
)
DELETE FROM consultation_sessions
WHERE case_id IN (SELECT id FROM seed_cases);

WITH seed_cases AS (
    SELECT id
    FROM clinical_cases
    WHERE full_name IN (
        'Paciente Ficticio 001',
        'Paciente Ficticio 002',
        'Paciente Ficticio 003'
    )
)
DELETE FROM case_question_overrides
WHERE case_id IN (SELECT id FROM seed_cases);

WITH seed_cases AS (
    SELECT id
    FROM clinical_cases
    WHERE full_name IN (
        'Paciente Ficticio 001',
        'Paciente Ficticio 002',
        'Paciente Ficticio 003'
    )
)
DELETE FROM case_physical_finding_overrides
WHERE case_id IN (SELECT id FROM seed_cases);

WITH seed_cases AS (
    SELECT id
    FROM clinical_cases
    WHERE full_name IN (
        'Paciente Ficticio 001',
        'Paciente Ficticio 002',
        'Paciente Ficticio 003'
    )
)
DELETE FROM case_question_answers
WHERE case_id IN (SELECT id FROM seed_cases);

WITH seed_cases AS (
    SELECT id
    FROM clinical_cases
    WHERE full_name IN (
        'Paciente Ficticio 001',
        'Paciente Ficticio 002',
        'Paciente Ficticio 003'
    )
)
DELETE FROM case_physical_findings
WHERE case_id IN (SELECT id FROM seed_cases);

DELETE FROM clinical_cases
WHERE full_name IN (
    'Paciente Ficticio 001',
    'Paciente Ficticio 002',
    'Paciente Ficticio 003'
);

COMMIT;
