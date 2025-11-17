#!/bin/bash
# 이 스크립트는 delete.sh의 날짜 기반 삭제 로직을 테스트하기 위해
# 특정 수정 날짜를 가진 샘플 파일과 디렉터리를 생성합니다.
set -e

echo "delete.sh 테스트를 위한 샘플 데이터 구조를 설정합니다..."

# delete.sh 스크립트가 대상으로 하는 기본 디렉터리
BASE_DIR="appdata/projects/sss/MYPROJ/airflow/logs"

# 테스트 실행 전, 이전 데이터를 삭제하여 깨끗한 상태에서 시작
echo "기존 샘플 데이터 정리..."
rm -rf "$BASE_DIR"
mkdir -p "$BASE_DIR"

# --- 삭제될 파일들 생성 ---
# 기준일(2025-06-01)보다 오래된 수정 날짜를 가집니다.

echo "삭제될 오래된 로그 파일 생성..."
# 7개월 전 파일
echo "이 로그는 2025년 4월의 기록입니다. 삭제 대상입니다." > "$BASE_DIR/old_log_april_2025.log"
touch -d "2025-04-15" "$BASE_DIR/old_log_april_2025.log"

# 삭제 기간의 마지막 날 파일
echo "이 로그는 2025년 5월 31일의 기록입니다. 삭제 대상입니다." > "$BASE_DIR/last_day_to_delete.log"
touch -d "2025-05-31" "$BASE_DIR/last_day_to_delete.log"

# 삭제될 파일만 담고 있어, 비워진 후 삭제될 디렉터리
mkdir -p "$BASE_DIR/dir_to_become_empty"
echo "이 중첩된 로그는 오래되었으므로 삭제됩니다." > "$BASE_DIR/dir_to_become_empty/nested_old_log.log"
touch -d "2025-03-20" "$BASE_DIR/dir_to_become_empty/nested_old_log.log"


# --- 유지될 파일들 생성 ---
# 기준일(2025-06-01)과 같거나 더 새로운 수정 날짜를 가집니다.

echo "유지될 최신 로그 파일 생성..."
# 보존 기간의 첫 날 파일
echo "이 로그는 2025년 6월 1일의 기록입니다. 유지 대상입니다." > "$BASE_DIR/first_day_to_keep.log"
touch -d "2025-06-01" "$BASE_DIR/first_day_to_keep.log"

# 현재 날짜의 파일
echo "이 로그는 오늘 날짜의 기록입니다. 유지 대상입니다." > "$BASE_DIR/current_log.log"
touch -d "$(date -I)" "$BASE_DIR/current_log.log"

# 유지될 파일을 담고 있어, 삭제되지 않을 디렉터리
mkdir -p "$BASE_DIR/dir_to_be_kept"
echo "이 중첩된 로그는 최신이므로 유지됩니다." > "$BASE_DIR/dir_to_be_kept/nested_kept_log.log"
touch -d "2025-08-10" "$BASE_DIR/dir_to_be_kept/nested_kept_log.log"


echo ""
echo "샘플 데이터 생성이 완료되었습니다."
echo "이제 'delete.sh --dry-run'을 실행하여 삭제될 파일 목록을 확인할 수 있습니다."
