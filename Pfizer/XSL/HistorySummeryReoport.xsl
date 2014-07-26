<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:decimal-format name="format" NaN="0" infinity="0" />
	<xsl:template match="/">
		<div id="tableContainer">
			<table align="Left" cellspacing="1" cellpadding="0" width="100%" id="MainTable" class="inner_tb" border="1">
				<thead>
          <tr class="tableHeader">
            <th >Calling Doctors</th>
            <th >Brick</th>
            <th >Speciality</th>
            <xsl:apply-templates mode="HMonth" select="//row[@Element = 'HMon']" />
          </tr>
				</thead>
				<xsl:choose>
					<xsl:when test="count(//row[@Element = 'Doc']) &gt;0">
						<tbody>
							<xsl:apply-templates mode="Doctor" select="//row[@Element = 'Doc']" />
						</tbody>
					</xsl:when>
					<xsl:otherwise>
						<tr class="tableContent1">
							<td align="center" class="alerttext" colspan="8">No Content Available</td>
						</tr>
					</xsl:otherwise>
				</xsl:choose>
			</table>
		</div>
	</xsl:template>
	
   <xsl:template match="//row[@Element = 'HMon']" mode="HMonth">
    <th><xsl:value-of select="@Mon" /></th>
    <th>Frq.</th>    
  </xsl:template>
    
  <xsl:template match="//row[@Element = 'Doc']" mode="Doctor">
		<xsl:variable name="DocPKID" select="@PKID" />
		<xsl:variable name="DocID" select="@PKID" />
		<tr>
			<xsl:if test="(position() mod 2)=0">
				<xsl:attribute name="class">tableContent1</xsl:attribute>
			</xsl:if>
			<xsl:if test="(position() mod 2)!=0">
				<xsl:attribute name="class">tableContent1</xsl:attribute>
			</xsl:if>
			<td align="left"  NOWRAP="TRUE" style="vertical-align:top">
				<xsl:value-of select="@DNam" />
			</td>
			<td align="left" NOWRAP="TRUE" style="vertical-align:top">
				<xsl:value-of select="@ANam" />
			</td>
			<td align="left" NOWRAP="TRUE" style="vertical-align:top">
				<xsl:value-of select="@DSpl" />
			</td>
      <xsl:apply-templates mode="DMonth" select="//row[@Element = 'HMon']" >
        <xsl:with-param name="doctor-id">
          <xsl:value-of select="$DocPKID" />
        </xsl:with-param>    
      </xsl:apply-templates>
		</tr>
	</xsl:template>

  <xsl:template match="//row[@Element = 'HMon']" mode="DMonth">
    <xsl:param name="doctor-id" />
    <xsl:variable name="p-year" select="@PYear" />
    <xsl:variable name="p-month" select="@PMonth" />
    <td>
      <xsl:apply-templates mode="Date" select="//row[@Element='Date' and @DocID=$doctor-id and @Year=$p-year and @Month=$p-month ]" />
    </td>
    <td align="right" NOWRAP="TRUE">
      <xsl:value-of select="//row[@Element='Freq' and @DocID=$doctor-id and @Year=$p-year and @Month=$p-month ]/@Freq" />
    </td>
  </xsl:template>

  <xsl:template mode="Date" match="//row[@Element = 'Date']" >
    <xsl:value-of select="@Date" /> <br />
  </xsl:template>
</xsl:stylesheet>
