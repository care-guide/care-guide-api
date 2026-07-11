#!/usr/bin/env python3
"""
Fails (non-zero exit) if any tracked appsettings*.json file contains a populated
secret field (JWT signing key, connection string with credentials, storage
secret keys, etc). Secrets must only live in `dotnet user-secrets` locally, or
in environment variables / a secrets vault in CI and production.

See docs/CONFIGURATION.md (Secrets) and docs/SECURITY.md (7. Secret management).

Usage:
    python3 scripts/check-secrets.py [file ...]

If no files are given, all tracked appsettings*.json files in the repo are
checked (via `git ls-files`).

IMPORTANT: this script must never print the offending value, only the file
and the key path, to avoid leaking secrets into hook/CI logs.
"""

from __future__ import annotations

import json
import subprocess
import sys

# Dotted paths (within the parsed JSON) that must never hold a non-empty
# string in tracked configuration.
SECRET_FIELD_PATHS = [
    ("ConnectionStrings", "DatabaseConnection"),
    ("SecuritySettings", "SecretKey"),
    ("Storage", "R2", "AccessKey"),
    ("Storage", "R2", "SecretKey"),
]


def tracked_appsettings_files() -> list[str]:
    result = subprocess.run(
        ["git", "ls-files", "*appsettings*.json"],
        capture_output=True,
        text=True,
        check=True,
    )
    return [line for line in result.stdout.splitlines() if line.strip()]


def get_path(data: dict, path: tuple[str, ...]):
    node = data
    for key in path:
        if not isinstance(node, dict) or key not in node:
            return None
        node = node[key]
    return node


def check_file(path: str, display_name: str | None = None) -> list[str]:
    """Returns a list of violation descriptions (key path only, never the value).

    `display_name` lets callers (e.g. the pre-commit hook, which checks a
    temp copy of the staged content) report the real repo-relative path
    instead of a temp file path.
    """
    label = display_name or path
    try:
        with open(path, "r", encoding="utf-8-sig") as f:
            content = f.read()
    except FileNotFoundError:
        return []

    if not content.strip():
        return []

    try:
        data = json.loads(content)
    except json.JSONDecodeError as exc:
        return [f"{label}: could not parse as JSON ({exc})"]

    violations = []
    for field_path in SECRET_FIELD_PATHS:
        value = get_path(data, field_path)
        if isinstance(value, str) and value != "":
            key_name = ":".join(field_path)
            violations.append(f"{label}: '{key_name}' is populated ({len(value)} chars)")

    return violations


def main(argv: list[str]) -> int:
    # Supports "path" or "path=Display/Name.json" pairs, so the pre-commit
    # hook can check a temp file while reporting the real repo path.
    args = argv[1:]
    files: list[tuple[str, str | None]]
    if args:
        files = []
        for arg in args:
            if "=" in arg:
                real_path, label = arg.split("=", 1)
                files.append((real_path, label))
            else:
                files.append((arg, None))
    else:
        files = [(f, None) for f in tracked_appsettings_files()]

    if not files:
        print("check-secrets: no appsettings*.json files found to check.")
        return 0

    all_violations: list[str] = []
    for path, label in files:
        all_violations.extend(check_file(path, label))

    if all_violations:
        print("check-secrets: BLOCKED - populated secret field(s) found in tracked configuration:")
        for violation in all_violations:
            print(f"  - {violation}")
        print()
        print("Secrets must never be committed with real values. Use `dotnet user-secrets` "
              "locally, or environment variables / a secrets vault in CI and production. "
              "See docs/CONFIGURATION.md (Secrets) and docs/SECURITY.md (7).")
        return 1

    print(f"check-secrets: OK - checked {len(files)} file(s), no populated secret fields.")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))
