# INSTRUCCIONES PARA SIGUIR LA CLASE 12

¡El código ya está actualizado! Sigue estos pasos en Unity:

---

## A. Puesta a punto (20 min)

### A.1 Verifica que el nivel funciona
1. Abre **Assets/Scenes/Nivel2D** y dale **Play**
2. Recorre el nivel 30 segundos
3. **Comprobación A:** ¿Se puede **ganar** (llegar a la meta) y **perder** (3 vidas)? Si no, anótalo para la práctica de la semana

---

## B. El menú principal (20 min)

### B.1 Crear la escena del menú
1. Unity → **File → New Scene** → Plantilla: **Basic 2D**
2. **File → Save As...** → Nombre: **`MenuPrincipal`** → Carpeta: **Assets/Scenes**

### B.2 Configurar el fondo
1. Selecciona **Main Camera** en la Hierarchy
2. Inspector → Component **Camera** → Sección **Environment**
3. En **Background Type** elige: **Solid Color**
4. Haz clic en el color → Elige **azul oscuro** o **negro** (a tu gusto)

### B.3 Crear el título del juego
1. Hierarchy → **Right-click** (o **Clic derecho**) → **UI → Text - TextMeshPro**
2. En el inspector, busca el campo **Text** y escribe: **`SALTA Y RECOGE`** (o el nombre de tu juego)
3. **Font Size**: **72**
4. Color: **blanco** o **amarillo**
5. En **Rect Transform**:
   - **Anchors**: arrástralo todo al **centro** (las esquinas deben encontrarse en el centro)
   - **Position Y**: **100**

### B.4 Crear el botón JUGAR
1. Hierarchy → **Right-click** → **UI → Button - TextMeshPro**
2. **Después de crear el botón**, haz esto:
   - En la Hierarchy verás dos objetos: **Button** y su hijo **Text (TMP)**
   - Selecciona el **Text (TMP)** (el segundo en la lista)
   - En Inspector → **Text** → Escribe: **`JUGAR`**
   - **Font Size**: **48**
   - En **Rect Transform**:
     - **Position Y**: **-50**
     - **Width**: **400**
     - **Height**: **100**

### B.5 Crear el objeto "Menu" (¡importante!)
1. En la ventana **Hierarchy** (panel izquierdo), haz **clic derecho** en un espacio vacío.
2. Selecciona **Create Empty**.
3. En la Hierarchy aparecerá un objeto llamado **"New GameObject"**. Haz clic derecho sobre él → **Rename** (o presiona F2) y escribe: **`Menu`**.
4. **Verifica**: debes ver el objeto **`Menu`** en la Hierarchy.

### B.6 Añadir el script MenuPrincipal al objeto Menu
1. Selecciona el objeto **`Menu`** en la Hierarchy.
2. En el Inspector (panel derecho) → **Add Component**.
3. Busca en el buscador: **`MenuPrincipal`** (el script que creamos al inicio).
4. Haz clic en **Add Component**.

### B.7 Conectar el botón JUGAR
1. Selecciona el **Button** (no el Text, el padre Button) en la Hierarchy.
2. Inspector → Sección **On Click ()** → Haz clic en **+**.
3. En el primer cuadro (Object):
   - Busca **Menu** en la Hierarchy (el objeto que creaste en el paso B.5).
   - **Arrástralo** desde la Hierarchy y suéltalo en ese cuadro.
4. En el segundo menú (Function): **MenuPrincipal → Jugar**.

**¡Listo el Menú!** Prueba: Dale **Play** en la escena `MenuPrincipal`, aprieta JUGAR y debe cargar el Nivel2D.

---

## C. Los dos botones en el nivel 1 (30 min)

### C.1 Configurar la Meta en el nivel 1
1. **No hagas nada en el código** - ya está listo
2. Abre **Assets/Scenes/Nivel2D**
3. Selecciona el objeto **`Meta`** en la Hierarchy
4. Inspector → **Meta (Script)**
5. En el campo **`Escena Siguiente`**, escribe: **`Nivel2D_2`**
   - ¡Importante! Con la mayúscula, sin tildes, sin espacios

### C.2 Crear el botón SIGUIENTE NIVEL
1. En la Hierarchy, busca **PanelVictoria** (debe estar **desactivado** = checkbox apagado)
2. **Haz clic en el checkbox** para activarlo (lo prende)
3. **Right-click** sobre **PanelVictoria** → **UI → Button - TextMeshPro**
4. En la Hierarchy ahora verás un **Button** y su hijo **Text (TMP)**
5. Selecciona el **Text (TMP)**
6. En Inspector → **Text** → Escribe: **`SIGUIENTE NIVEL`**
7. **Font Size**: **40**
8. En **Rect Transform**:
   - **Position Y**: **-80** (debajo del texto ¡GANASTE!)
   - **Width** y **Height**: puedes dejarlos como están
9. Selecciona ahora el **Button** (el padre, no el Text)
10. Inspector → **On Click ()** → **+**
11. En el primer cuadro (Object):
    - Busca **Meta** en la Hierarchy (el objeto que ya existe en el nivel)
    - Arrástralo al cuadro
12. En el segundo menú (Function): **Meta → SiguienteNivel**
13. **Haz clic en el checkbox de PanelVictoria para desactivarlo** de nuevo

### C.3 Crear el botón REINTENTAR
1. En la Hierarchy, busca **PanelGameOver** (debe estar **desactivado**)
2. **Haz clic en el checkbox** para activarlo
3. **Right-click** sobre **PanelGameOver** → **UI → Button - TextMeshPro**
4. Selecciona el **Text (TMP)**
5. En Inspector → **Text** → Escribe: **`REINTENTAR`**
6. **Font Size**: **40**
7. En **Rect Transform**:
   - **Position Y**: **-80**
8. Selecciona ahora el **Button** (el padre)
9. Inspector → **On Click ()** → **+**
10. En el primer cuadro (Object):
    - Busca **Jugador** en la Hierarchy
    - Arrástralo al cuadro
11. En el segundo menú (Function): **Recolector2D → Reintentar**
12. **Haz clic en el checkbox de PanelGameOver para desactivarlo** de nuevo

### C.4 Probar REINTENTAR
1. Guarda la escena (**Ctrl + S** / **Cmd + S**)
2. Dale **Play** en Nivel2D
3. Déjate golpear por el enemigo **3 veces**
4. Cuando aparezca **GAME OVER**, presiona el botón **REINTENTAR**
5. **Comprobación:** El nivel debe recargar fresco:
   - Jugador en el inicio
   - Vidas de vuelta a 3
   - Monedas en su sitio
   - El juego **descongelado**

---

## D. El nivel 2 (15 min)

### D.1 Crear la copia del nivel
1. Abre el **Panel de Escenas** en Unity (ventana con las miniaturas de escenas)
2. **Clic derecho** sobre **Nivel2D** → **Duplicate**
3. Renombra la copia: **`Nivel2D_2`** (F2)
4. **Comprobación D.1:** Ahora en el Panel de Escenas verás **Nivel2D** y **Nivel2D_2**

### D.2 Configurar la meta del nivel 2
1. **Abre** la escena **`Nivel2D_2`** (doble clic o selección en el panel)
2. Selecciona el objeto **`Meta`**
3. Inspector → **Meta (Script)**
4. En el campo **`Escena Siguiente`**:
   - **Escribe: `MenuPrincipal`** (¡no borres lo que dice ahora, cámbialo!)
   - ¡Importante! Con la mayúscula, sin tildes, sin espacios

### D.3 (Opcional) Hacer el nivel 2 diferente
Elige **dos** de estas:
- Mover la `Meta` a otro lugar del nivel
- Agregar más monedas (Drag and drop desde Assets/Prefabs/Moneda2D)
- Duplicar el enemigo y poner el clon en otra posición

### D.4 Guardar
1. **Ctrl + S** en la escena `Nivel2D_2`

---

## E. El circuito completo y el .exe (15 min)

### E.1 Configurar Build Settings
1. Unity → **File → Build Settings**
2. En la lista de escenas (**Scenes in build**), deben estar las 3:
   - **MenuPrincipal**
   - **Nivel2D**
   - **Nivel2D_2**
3. Si falta alguna:
   - Selecciona la que falta en el panel de escenas (arriba a la izquierda)
   - Haz clic en **Add Open Scenes**
4. **Arriba de la lista:**
   - Arrastra **MenuPrincipal** para que quede **primero** (índice 0)

### E.2 Probar el circuito completo
1. **Abre** escena **`MenuPrincipal`**
2. Dale **Play**
3. Recorre todo el circuito:
   - Acierta el botón **JUGAR**
   - Juega el Nivel 1 hasta llegar a la meta
   - Aparece **¡GANASTE!** → acierta el botón **SIGUIENTE NIVEL**
   - Juega el Nivel 2 hasta llegar a la meta
   - Aparece **VOLVER AL MENÚ** (o **VOLVER AL MENÚ** si lo cambiaste) → acierta el botón
   - Vuelve al Menú Principal
4. **Comprobación E.2:** ¡El circuito funciona de punta a punta!

### E.3 Crear el .exe
1. **File → Save** (guarda la escena que tienes abierta)
2. Unity → **File → Build Settings**
3. Pestaña: **Windows** (o **Windows Processor 64-bit** en versiones nuevas)
4. Haz clic en **Build**
5. Elegí:
   - **Carpet:** El Escritorio
   - **Name:** **`SaltaYRecoge_v2`**
6. Haz clic en **Build** y espera (puede tardar unos minutos)
7. Ve al Escritorio y **abre el .exe** `SaltaYRecoge_v2.exe`
8. ¡Juega el circuito completo!
9. **Comprobación E.3:** 
   - El .exe se abre en el **Menú**
   - El circuito se juega completo (2 niveles)
   - Puedes **REINTENTAR** si pierdes

**🎉 ¡Felicidades! Has completado el ejercicio de hoy.**

---

## F. Opcional - Si sobra tiempo

### F.1 Agregar música en el menú
1. En la escena **MenuPrincipal**
2. **Right-click** → **Create Empty** → **Audio**
3. **Add Component** → **Audio Source**
4. Arrastra un archivo de música (mp3/wav)
5. En el Audio Source:
   - **Play On Awake**: ✅ activado
   - **Loop**: ✅ activado
   - **Volume**: **0.4**
   - **Spatial Blend**: **0**

### F.2 Botón SALIR
1. En la escena **MenuPrincipal**
2. **Right-click** sobre el objeto **Menu**
3. **Add Component** → Busca **MenuPrincipal**
4. Si no aparece, busca el script y **Add**
5. En Inspector, busca el script y arrástralo si está vacío
6. **Add Component** → **MenuPrincipal**
7. O edita el script **MenuPrincipal.cs**:
   ```csharp
   public void Salir()
   {
       Application.Quit();
   }
   ```
8. Guarda el script y vuelve a Unity
9. Selecciona el objeto **Menu**
10. **Add Component** → **MenuPrincipal** (si no está)
11. Crea otro botón en el menú:
    - Right-click → UI → Button
    - Text → **`SALIR`**
    - On Click () → **+**
    - Object: **Menu**
    - Function: **MenuPrincipal → Salir**
12. ¡Importante! El botón SALIR solo funciona en el **.exe**, no en Unity

---

## Checklist de terminación

Marca cuando todo esté listo:

- [ ] MenuPrincipal existe en el proyecto
- [ ] El Menú tiene título y botón JUGAR
- [ ] El botón JUGAR carga Nivel2D
- [ ] El nivel 1 tiene botones SIGUIENTE NIVEL y REINTENTAR
- [ ] Existe el nivel 2 (Nivel2D_2)
- [ ] El nivel 2 tiene la meta configurada para volver al menú
- [ ] La Build Settings tiene las 3 escenas con MenuPrincipal primero
- [ ] El .exe se abre en el menú
- [ ] El circuito completo funciona (2 niveles + menú)
- [ ] El game over permite reintentar

**¡Cuando marques las 10 casillas: ¡TERMINASTE LA CLASE 12!**