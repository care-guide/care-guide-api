#!/usr/bin/env sh
# Installs the repo's tracked git hooks (scripts/hooks/*) into .git/hooks,
# since .git/hooks itself is not versioned by git.
#
# Usage:
#   sh scripts/install-hooks.sh
#
# Run this once after cloning. See docs/CONFIGURATION.md (Secrets) for details
# on what the pre-commit hook enforces.

set -e

REPO_ROOT="$(git rev-parse --show-toplevel)"
HOOKS_SRC="$REPO_ROOT/scripts/hooks"
HOOKS_DEST="$(git rev-parse --git-path hooks)"

if [ ! -d "$HOOKS_SRC" ]; then
  echo "install-hooks: source directory $HOOKS_SRC not found." >&2
  exit 1
fi

mkdir -p "$HOOKS_DEST"

for HOOK in "$HOOKS_SRC"/*; do
  NAME=$(basename "$HOOK")
  cp "$HOOK" "$HOOKS_DEST/$NAME"
  chmod +x "$HOOKS_DEST/$NAME"
  echo "install-hooks: installed $NAME"
done

echo "install-hooks: done."
