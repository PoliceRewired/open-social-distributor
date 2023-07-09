# open-social-distributor

## TODOs

- [ ] Import read and prepare code
- [ ] Modify publish code

## Build and deploy scripts

* `open-social-distributor-app/export-defaults.sh` - defaults for `PROFILE` (AWS CLI), and `STACK_NAME` (CloudFormation stack)
* `open-social-distributor-app/build-functions.sh` - builds all functions in this project
* `open-social-distributor-app/dev-sync-app.sh` - synchronises all functions in this project with an AWS CloudFormation stack

## Testing scripts

* `open-social-distributor-app/build-unit-tests.sh` - builds all unit tests in this project
* `open-social-distributor-app/run-unit-tests.sh` - runs all unit tests in this project
