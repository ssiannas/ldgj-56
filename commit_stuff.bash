git branch temp-branch
git checkout temp-branch
git add .
git commit -m "$1"
git checkout main
git fetch
git pull origin main
git merge temp-branch
