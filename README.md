## 1. 프로젝트 개요

플레이어와 AI가 덱에서 카드를 뽑아 3장을 선택하고 전투를 진행하는 싱글플레이 턴제 전략 게임입니다. 

전투는 상태 기반 턴 시스템으로 구성되며, AI는 FSM + Behavior Tree를 활용해 전략적 선택을 수행합니다.

## 2. 기술 스택

-   **Engine** : Unity 2022.3.62f
-   **Language** : C#
-   **Architecture**
    -   Domain--Presentation 구조
    -   상태 패턴(State Pattern) 기반 턴 시스템
    -   ScriptableObject 데이터 중심 구조
    -   Addressables 기반 비동기 리소스 로딩
    -   FSM + Behavior Tree AI

## 3. 주요 특징

### ① Domain--Presentation Architecture

-   게임 규칙(Domain)과 UI·입력(Presentation)을 분리
-   유지보수성 및 확장성을 고려한 설계

### ② 상태 패턴 기반 턴 시스템

-   Draw → Select → Resolve → EndTurn 단계 분리
-   StateMachine이 전투 흐름을 관리

### ③ ScriptableObject + Addressables

-   데이터 중심 구조(Domain 독립성 강화)
-   리소스 비동기 로딩 및 메모리 최적화

## 6. 개발 기간 및 범위

**총 2주 개발(예상)**
- 1주차: 전투 뼈대 제작, 상태 머신, 기본 전투 루프
- 2주차: AI, ScriptableObject 데이터화, Addressables 도입, 리팩터링

## 7. 포트폴리오 목적

-   전투 로직 구조화 능력 강조
-   상태 패턴·도메인 분리·AI 설계 등 클라이언트 개발 핵심 기술 증명
-   실제 플레이 가능한 프로토타입 제공
