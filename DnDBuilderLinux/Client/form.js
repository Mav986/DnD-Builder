document.write('<form id="submissionForm">\n' +
'Name: <input required name="name" id="nameInput" type="text" disabled>\n' +
'    Race: <select required name="race" id="allRaces"></select>\n' +
'    Class: <select required name="class" id="allClasses" onchange="getSpellcaster()"></select>\n' +
'    <br>\n' +
'    <br>\n' +
'    Gender: <input name="gender" id="genderInput" type="text" maxlength="15">\n' +
'    Age: <input required name="age" id="ageInput" type="number" min="0" max="500" step="1" value="18">\n' +
'    Level: <input required name="level" id="levelInput" type="number" min="1" max="20" step="1" value="1"><br><br>\n' +
'    Spellcaster: <input id="spellInput" type="text" disabled>\n' +
'    Hitpoints: <input name="hitpoints" id="hpInput" type="text" disabled> \n' +
'    <br>\n' +
'    <br>\n' +
'    Ability Scores - Total points remaining: <div id="abilityScoreRem" style="display: inline">20</div>\n' +
'    <br>\n' +
'    <br>\n' +
'    Con: <input name="con" id="conInput" type="number" min="0" value="0" onchange="updateRemainingScore()">\n' +
'    Dex: <input name="dex" id="dexInput" type="number" min="0" value="0" onchange="updateRemainingScore()">\n' +
'    Str: <input name="str" id="strInput" type="number" min="0" value="0" onchange="updateRemainingScore()">\n' +
'    <br>\n' +
'    Cha: <input name="cha" id="chaInput" type="number" min="0" value="0" onchange="updateRemainingScore()">\n' +
'    Int: <input name="intel" id="intInput" type="number" min="0" value="0" onchange="updateRemainingScore()">\n' +
'    Wis: <input name="wis" id="wisInput" type="number" min="0" value="0" onchange="updateRemainingScore()">\n' +
'    <br>\n' +
'    <br>\n' +
'    Biography<br><textarea name="bio" rows="10" cols="70" maxlength="500"></textarea>\n' +
'    <br>\n' +
'    \n' +
'</form>');