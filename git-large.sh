# Do not run this script, just copy and paste the lines as-needed.

# list large objects in the repo, smallest to largest.
# this depends on numfmt. On MacOS, run `brew install coreutils`, or omit the last line.
git rev-list --objects --all \
| git cat-file --batch-check='%(objecttype) %(objectname) %(objectsize) %(rest)' \
| awk '/^blob/ {print substr($0,6)}' \
| sort --numeric-sort --key=2 \
| cut --complement --characters=13-40 \
| numfmt --field=2 --to=iec-i --suffix=B --padding=7 --round=nearest

# use this command to remove specific files
git filter-branch --index-filter 'git rm --cached --ignore-unmatch a b' HEAD
