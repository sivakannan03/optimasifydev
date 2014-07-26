<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:include href="PagingTemplate.xsl" />
<xsl:template match="/">
	<div id="tableContainer" >
		<xsl:choose>
			<xsl:when test ="count(//row[@Element='cmd']) &gt;0">
				<xsl:for-each select="//row[@Element='PSOName']">	
				<xsl:variable name="ChdPSOFKID" select="@PSOFKID" />					
				<table  align="center" id="tblmain" width="100%" cellspacing="1" cellpadding="0"  class="inner_tb"  border="1" >
					<tr class="tableContent1" align="left">
						<th class="tablecontentRep" width = "25%">Name </th>
						<td class = "tablecontent2" colSpan="6" width = "25%">
						<xsl:value-of select="@psoName" />
						</td>
					</tr>
				</table>		
				<table  align="center" id="tblresult" width="100%" cellspacing="1" cellpadding="0"  class="inner_tb"  border="1" >
					<tr class="tableContent1" id = "Prod">
						<th class="tableheader" width="30%" align="left">Day</th>
						<th class="tableheader" width="20%" align="center">Date</th>
						<th class="tableheader" width="20%" align="center">Status</th>
					</tr>
						<xsl:choose>
							<xsl:when test ="count(//row[@Element='cmd' and @PsoFKID=$ChdPSOFKID]) &gt;0">
								<xsl:apply-templates select="//row[@Element='cmd' and @PsoFKID=$ChdPSOFKID]"/>
							</xsl:when>
							<xsl:otherwise>
								<tr class="tablecontent1" align="center"> 
									<td align="center" class="alerttext" colspan="10" >No Records Found</td>
								</tr>
							</xsl:otherwise>
						</xsl:choose>
				</table>				
					<br>
					</br> 
				</xsl:for-each>								
			</xsl:when>
			<xsl:otherwise>
						<table  align="center" id="tblmain" width="100%" cellspacing="1" cellpadding="0"  class = "tablebg" >
						<tr class="tableContent1" id = "Prod">
							<td class="tableheader" width="30%" align="left">Day</td>
							<td class="tableheader" width="20%" align="center">Date</td>
							<td class="tableheader" width="20%" align="center">Status</td>
						</tr>
						<tr class="tablecontent1" align="center"> 
							<td align="center" class="alerttext" colspan="10" >No Records Found For All PSOs</td>
						</tr>
						</table>
			</xsl:otherwise>
		</xsl:choose>
	</div>
	</xsl:template>
	<xsl:template match="row[@Element='cmd']">
	<div>
		<tr class="tablecontent2" id = "child">
								
			<td class="text1">
			<font class="text3">
			
			<xsl:if test="@LeaveDays='1'" >
						<xsl:attribute name="color">#ff0000</xsl:attribute>
			</xsl:if>
			
			<xsl:if test="@LeaveDays='2'" >
						<xsl:attribute name="color">#c98d00</xsl:attribute>
			</xsl:if>
			
			<xsl:if test="@LeaveDays='3'" >
						<xsl:attribute name="color">#9933cc</xsl:attribute>
			</xsl:if>
					
			<xsl:value-of select="@days" />
			
			</font>
			</td>

			<td class="text1">
					<xsl:value-of select="@Date" />
			</td>
			
			<td class="text1">
					<xsl:value-of select="@SaveFlag" />
			</td>
			
		</tr>
		</div>
	</xsl:template>
</xsl:stylesheet>