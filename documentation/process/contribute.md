# Contribute

## tldr

1. [Go to the issue or create a new issue](https://code.fki.htw-berlin.de/cm/studierendenprojekte/bp-ss25-b5-urban-bio-space-simulator/-/issues).
   Remember the issue number and copy the issue title.
   In this example: number **\#123**, title **Create the new awesome cricket-cruncher**.
2. Create branch
   ```shell
   git switch main
   git pull
   git switch -c 123-UBSS --track origin/123-UBSS
   ```
3. Commit
   ```shell
   ... hack hack hack ...
   git add cricket-cruncher
   git commit -m "GL-123 Create the new awesome cricket-cruncher"
   git push
   ```
4. [Create a merge request](https://code.fki.htw-berlin.de/cm/studierendenprojekte/bp-ss25-b5-urban-bio-space-simulator/-/merge_requests/new)
   Source branch: `123-UBSS`
   Target branch: `main`
   
## Branch

The branch for the issue `#123 Create the new awesome cricket-cruncher` is called `123-UBSS`. (happy to bikeshed about the UBSS)

To get a number, [create a new issue](https://code.fki.htw-berlin.de/cm/studierendenprojekte/bp-ss25-b5-urban-bio-space-simulator/-/issues) before creating a branch.
If the issue has `#123`, then the branch should be called `123-UBSS` This is because gitlab can autolink a branch that matches `123-*` to the issue with number `#123`[^gitlab-crosslinking-branch]

### main

TBD

### 123-UBSS

This would be the branch for issue **\#123** for **U**rban **B**io **S**pace **S**imulator

## Commit

A commit for the issue `#123 Create the new awesome cricket-cruncher` needs to start with `GL-123`[^gitlab-crosslinking-commit].

The lazy way is to just copy the issue title and commit like this:

```shell
git commit -m "GL-123 Create the new awesome cricket-cruncher"
```

Split it into multiple commits like this:

```shell
git commit -m "GL-123 Create the new awesome cricket-cruncher - refactor something"
```

```shell
git commit -m "GL-123 Create the new awesome cricket-cruncher - add cruncer"
```

```shell
git commit -m "GL-123 Create the new awesome cricket-cruncher - documentation"
```

Or do something else, but it needs to start with `GL-123`

## Merge Request

The merge request can only be merged if the branch is based on the latest commit in the main branch.

The commits in the merge request will be squashed automatically.
That way, each issue references one commit on the main branch.

If you start your work based on the main branch and someone else has their branch merged before you, you need to rebase.
In some cases you can just rebase in gitlab.
If not, gitlab will tell you.

### Rebase manually to solve conflicts

- If you mess up or are not sure, just go back to before the rebase: `git rebase --abort`
- You should configure this, it solves the only big downside of rebaseing over merging: `git config --global rerere.enabled true`

1. Go to your own branch if you are not already there.
   ```shell
   git switch 123-UBSS
   ```
2. Rebase onto the latest commit on branch main.
   ```shell
   git pull origin main --rebase
   ```
3. Solve your confilcts
   You can do this in your IDE, it should be visible after the previous command.
4. After solving the conflicts, you can continue to the next conflict or end the rebase.
   ```shell
   git rebase --continue
   ```
5. Now you need to push to your branch `origin/123-UBSS`. Due to the rebase, you need to force the update.
   ```shell
   git push --force-with-lease
   ```

## Notes

[^gitlab-crosslinking-commit]: 
    https://docs.gitlab.com/user/project/issues/crosslinking_issues/#from-commit-messages
[^gitlab-crosslinking-branch]:
    https://docs.gitlab.com/user/project/repository/branches/#prefix-branch-names-with-a-number
