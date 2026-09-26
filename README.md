# Albedolizer PBR Generator for Unity

Generate production-ready PBR maps from Albedo textures — right inside Unity Editor.

**Free & open-source (MIT). Donations welcome.**

---

## 🇬🇧 English

### What it does

- **7 PBR maps** from a single Albedo texture: Height, Normal, AO, Roughness, Metallic, Edge, ORM
- **AI color correction** — Autolevels or LUTwithBGrid
- **Math fallback** — CLAHE + soft-clip (no AI required)
- **50 material presets** with auto-detect from filename
- **Seamless generation** — GIMP tile-seamless algorithm
- **Unity URP material** — auto-created with proper channels (R=Metallic, G=AO, A=Smoothness)
- **HDRP / Unreal / Godot packing** — also supported
- **Works offline** — CLI is bundled inside the package

### Installation

**Option A — UPM via Git URL (recommended):**

1. Open `Window → Package Manager`
2. Click `+` → **Add package from git URL...**
3. Paste:
https://github.com/invisiblelevel/Albedolizer-Unity.git

text
4. Unity downloads and installs the package automatically
5. `Window → Albedolizer` opens the add-on

**Option B — Add package from disk:**

1. Clone this repo or download the release zip
2. `Window → Package Manager` → `+` → **Add package from disk...**
3. Pick `com.invisiblelevel.albedolizer/package.json`

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


### About Albedolizer

This add-on is the Unity bridge for **Albedolizer** — a free desktop tool for 3D artists:

- 🌐 **GitHub**: [github.com/invisiblelevel/Albedolizer](https://github.com/invisiblelevel/Albedolizer)
- 🎮 **itch.io**: [invlvl.itch.io/albedolizer](https://invlvl.itch.io/albedolizer)
- 🅱️ **Blender add-on**: [Albedolizer-Blender](https://github.com/invisiblelevel/Albedolizer-Blender)

### License

Add-on code: **MIT**
Bundled CLI binaries: proprietary.

---

## 🇷🇺 Русский

### Что делает

- **7 PBR-карт** из одной Albedo-текстуры: Height, Normal, AO, Roughness, Metallic, Edge, ORM
- **AI-коррекция цвета** — Autolevels или LUTwithBGrid
- **Математический fallback** — CLAHE + soft-clip
- **50 пресетов материалов** с автоопределением из имени файла
- **Бесшовность** — GIMP tile-seamless
- **Автосоздание URP-материала** с правильными каналами (R=Metallic, G=AO, A=Smoothness)
- **Упаковка под HDRP / Unreal / Godot**
- **Работает офлайн** — CLI встроен в пакет

### Установка

**Способ А — UPM через Git URL (рекомендую):**

1. Открой `Window → Package Manager`
2. Кнопка `+` → **Add package from git URL...**
3. Вставь:
https://github.com/invisiblelevel/Albedolizer-Unity.git

text
4. Unity сама скачает и поставит пакет
5. `Window → Albedolizer` откроет окно

**Способ Б — Add package from disk:**

1. Склонируй репу или скачай релизный zip
2. `Window → Package Manager` → `+` → **Add package from disk...**
3. Выбери `com.invisiblelevel.albedolizer/package.json`

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

### Про Albedolizer

- 🌐 **GitHub**: [github.com/invisiblelevel/Albedolizer](https://github.com/invisiblelevel/Albedolizer)
- 🎮 **itch.io**: [invlvl.itch.io/albedolizer](https://invlvl.itch.io/albedolizer)
- 🅱️ **Blender-аддон**: [Albedolizer-Blender](https://github.com/invisiblelevel/Albedolizer-Blender)

### Лицензия

Код аддона: **MIT**
Встроенные CLI-бинарники: проприетарные.

---

## License / Лицензия

Add-on code: **MIT** · Bundled CLI binaries: proprietary.
Код аддона: **MIT** · Встроенные CLI-бинарники: проприетарные.