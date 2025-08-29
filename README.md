# The Mule — Personal Project (Avalonia .NET, MVVM) for Printed T-Shirts  
**Printify → Shopify** semi-automation desktop tool

**The Mule** is a **personal project**: an Avalonia desktop app (MVVM) that streamlines a **printed T-shirt** workflow. It generates mock product images and items via the **Printify API**, and can (optionally) create the **matching Shopify products** that use those images.

---

## ✨ Why / Scope (T-Shirt Focus)

This tool is opinionated toward **printed T-shirts**:
- Step-by-step flow to generate **mock images** for a design across common **sizes/variants** (e.g., S–XXL, multiple colors where supported)
- Batch creation of **Printify items** using those mockups
- **Optional** posting of **Shopify products** that reference the generated assets

The goal: turn a multi-step, error-prone process into a **guided desktop workflow** with clear progress and diagnostics.

---

## 🧠 What it does (high-level)

1. **Authenticate with Printify** using your API key  
2. **Select a T-shirt base** and **design**; prepare variant payloads  
3. **Generate mock images** (mockups) for chosen colours  
5. **Create/Update Printify items** with those images  
6. **Create Shopify products** using those generated assets

---

## 🏗️ Tech

- **.NET** + **C#**
- **Avalonia UI** (cross-platform XAML) with **MVVM** (commands, bindings)
- **HTTP API clients** for Printify and Shopify

---

---

## 🖼️ Screenshots

<p align="center">
  <img src="screenshots/Screenshot%202025-08-29%20174009.png" alt="Printify artworks" width="720"/>
</p>
<p align="center">
  <img src="screenshots/Screenshot%202025-08-29%20174326.png" alt="Printify products" width="720"/>
</p>
<p align="center">
  <img src="screenshots/Screenshot%202025-08-29%20174354.png" alt="Printify mockup generation settings" width="720"/>
</p>

---

⚖️ Disclaimer

This is a personal project for demonstration/education.
Not affiliated with Printify or Shopify. Please respect their API terms and rate limits.
