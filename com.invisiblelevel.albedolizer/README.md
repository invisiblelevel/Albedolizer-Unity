# Albedolizer PBR Generator for Unity

Generate production-ready PBR maps from Albedo textures — right inside Unity Editor.

**Free & open-source (MIT). Donations welcome.**

---

## 🇬🇧 English

### What it does

- **7 PBR maps** from a single Albedo texture: Height, Normal, AO, Roughness, Metallic, Edge, ORM
- **AI color correction** — Autolevels or LUTwithBGrid
- **Math fallback** — CLAHE + soft-clip (no AI required)
- **50 material presets** — metal, brick, wood, concrete, fabric, fauna, and more
- **Auto-detect preset from filename** — `brick_wall.png` → preset `brick`
- **Seamless generation** — GIMP tile-seamless algorithm
- **Unity URP material** — auto-created with proper channels (R=Metallic, G=AO, A=Smoothness)
- **HDRP / Unreal / Godot packing** — also supported
- **Works offline** — CLI is bundled inside the package

### Installation

**Option A — UPM (recommended):**

1. `Window → Package Manager`
2. Click `+` → **Add package from disk...**
3. Pick `package.json` from this package
4. `Window → Albedolizer` opens the add-on

**Option B — .unitypackage:**

1. `Assets → Import Package → Custom Package...`
2. Pick the `.unitypackage` file

### Usage

1. Open `Window → Albedolizer`
2. Pick your **Albedo texture** (from anywhere on disk)
3. Choose preset, correction mode, maps
4. Select **Engine Packing → Unity URP**
5. Click **Generate PBR**

The maps are copied to `Assets/Albedolizer_Output/<name>/` and a URP material is auto-created.

### Requirements

- Unity **2022.3 LTS** or newer (tested on Unity 6)
- **Universal Render Pipeline (URP)** installed
- **Windows 10/11 (64-bit)**
- No external tools needed — CLI is bundled

### About Albedolizer

This add-on is the Unity bridge for **Albedolizer** — a free desktop tool for 3D artists:

- 🌐 **GitHub**: [github.com/invisiblelevel/Albedolizer](https://github.com/invisiblelevel/Albedolizer)
- 🎮 **itch.io**: [invlvl.itch.io/albedolizer](https://invlvl.itch.io/albedolizer)

### License

Add-on code: **MIT**
Bundled CLI binaries: proprietary.

---

## 🇷🇺 Русский

### Что делает

- **7 PBR-карт** из одной Albedo-текстуры
- **AI-коррекция цвета** (Autolevels / LUTwithBGrid)
- **Математический fallback** (CLAHE)
- **50 пресетов материалов** с автоопределением из имени файла
- **Бесшовность** (GIMP tile-seamless)
- **Автосоздание URP-материала** с правильными каналами (R=Metallic, G=AO, A=Smoothness)
- **Упаковка под HDRP / Unreal / Godot** — тоже есть
- **Работает офлайн** — CLI встроен в пакет

### Установка

**Способ А — UPM (рекомендую):**

1. `Window → Package Manager`
2. Кнопка `+` → **Add package from disk...**
3. Выбери `package.json` из этого пакета
4. `Window → Albedolizer` откроет окно

**Способ Б — .unitypackage:**

1. `Assets → Import Package → Custom Package...`
2. Выбери `.unitypackage`

### Как пользоваться

1. `Window → Albedolizer`
2. Выбери **Albedo-текстуру** (откуда угодно)
3. Пресет, режим коррекции, карты
4. **Engine Packing → Unity URP**
5. **Generate PBR**

Карты копируются в `Assets/Albedolizer_Output/<имя>/` и создаётся URP-материал.

### Требования

- Unity **2022.3 LTS** или новее (тестировано на Unity 6)
- Установлен **Universal Render Pipeline (URP)**
- **Windows 10/11 (64-bit)**
- Больше ничего — CLI встроен


### Про Albedolizer

Полная десктопная прога — бесплатная, MIT:

- 🌐 **GitHub**: [github.com/invisiblelevel/Albedolizer](https://github.com/invisiblelevel/Albedolizer)
- 🎮 **itch.io**: [invlvl.itch.io/albedolizer](https://invlvl.itch.io/albedolizer)

### Лицензия

Код аддона: **MIT**
Встроенные CLI-бинарники: проприетарные.

---

## License / Лицензия

Add-on code: **MIT** · Bundled CLI binaries: proprietary.
Код аддона: **MIT** · Встроенные CLI-бинарники: проприетарные.