#!/usr/bin/env bash

set -euo pipefail

pushd ${CI_PROJECT_DIR}/development/ci

export JSON_KEY_FILE=./fastlane-json-key.json

gem install bundler
bundle install

mkdir -p ./fastlane/metadata/android/en-US/changelogs
#echo "No release notes." > ./fastlane/metadata/android/en-US/changelogs/default.txt
git log -n 1 | tail -n +3 > ./fastlane/metadata/android/en-US/changelogs/${CI_PIPELINE_IID}.txt

echo ${ANDROID_FASTLANE_BASE64} | base64 -d > $JSON_KEY_FILE

bundle exec fastlane supply \
    --track alpha \
    --aab "${CI_PROJECT_DIR}/Builds/Android/${BUILD_NAME}.${ANDROID_BUILD_TYPE}" \
    --json-key $JSON_KEY_FILE \
    --package_name "com.abductedrhino.biotopia" \
    --metadata_path ./fastlane/metadata/android

rm -f $JSON_KEY_FILE

popd
