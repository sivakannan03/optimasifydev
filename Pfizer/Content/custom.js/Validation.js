function SingleCharacter(s) {
    s.value = (s.value.replace(/(\s{2,})|[^a-zA-Z']/g, ' '));
    s.value = (s.value.replace(/^\s*/, ''));
    return;
}
function OnlyNumber(s) {
    s.value = (s.value.replace(/(\s{2,})|[^0-9]/g, ' '));
    s.value = (s.value.replace(/^\s*/, ''));
    return;
}
function OnlyNumberPoints(s) {
    s.value = (s.value.replace(/(\s{2,})|[^0-9.,]/g, ' '));
    s.value = (s.value.replace(/^\s*/, ''));
    return;
}

function AlphaNumberPoints(s) {
    s.value = (s.value.replace(/(\s{2,})|[^0-9a-zA-Z]/g, ' ')); 
    s.value = (s.value.replace(/^\s*/, ''));
    return;
}

function CharacterAndNumber(s) {
    s.value = (s.value.replace(/(\s{2,})|[^a-zA-Z0-9']/g, ' '));
    s.value = (s.value.replace(/^\s*/, ''));
    return;
}


function email(s) {
    s.value = (s.value.replace(/(\s{2,})|[^a-zA-Z0-9.@,]/g, ' '));
    s.value = (s.value.replace(/^\s*/, ''));
    return;
}

function SingleCharacterQustion(s) {
    s.value = (s.value.replace(/(\s{2,})|[^a-zA-Z?']/g, ' '));
    s.value = (s.value.replace(/^\s*/, ''));
    return;
}