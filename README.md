# LCKR
## Description
Translate the in-game language into Korean. It's very hardcoded and can cause problems!

게임 내 언어를 한글로 변경합니다. 그냥 한패롤 모드로 옮긴 거라고 생각하시면 편합니다 (기존 한패와 거의 동일함)

## 구성 설명

### 폰트 변경
- 기본값은 true입니다.
- FontPatcher 등 외부 폰트 모드를 사용하려면 이 값을 true로 설정하세요. 이 값을 false로 설정한다면 바닐라 폰트로 적용되며, 한글이 깨져보일 수도 있습니다.

### 행성 내부 이름 번역
- 기본값은 false입니다.
- 코드에서 사용되는 행성의 내부 이름을 한글화합니다. 게임 플레이에서 달라지는 부분은 없지만, true로 하면 모드 인테리어의 구성을 변경할 때 행성 명을 한글로 입력해야 합니다. false로 두면 그대로 영어로 입력하시면 됩니다.

### Thumper 번역
- 기본값은 false입니다.
- true로 설정하면 "Thumper"를 썸퍼로 번역합니다. false로 설정하면 덤퍼로 설정됩니다.

### KG 변환
- 기본값은 true입니다.
- true로 설정하면 무게 수치를 kg으로 변환합니다.

## 크레딧

- 게임 기본 폰트 - [Orbit체](https://github.com/JAMO-TYPEFACE/Orbit)
- 신호 해석기 폰트 - [갈무리9](https://galmuri.quiple.dev/)
